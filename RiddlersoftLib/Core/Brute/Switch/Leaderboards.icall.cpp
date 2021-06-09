//---------------------------------------------------------------------------
//SORRY! This code yaks about NN_DEPRECATED
//But it's lotcheck-tested... There's no sense changing anything until we need to (checked on nx sdk 7.3.0)
//---------------------------------------------------------------------------
#include <mscorlib/1nternal/Brute.h>
#include "Riddlersoft.Core/Riddlersoft/Core/Networking/Leaderboards.h"
#include "Riddlersoft.Core/Riddlersoft/Core/Networking/HighScore.h"
#include "Riddlersoft.Core/Riddlersoft/Core/Networking/Leaderboards+Ranking2ScoreUploadRecord.h"
#include "Riddlersoft.Core/Riddlersoft/Core/Networking/LeaderboardFilter.h"
#include "mscorlib/System/Exception.h"
#include "mscorlib/System/String.h"
#include "mscorlib/System/String.inl"

//---------------------------------------------------------------------------
#include <stdint.h>

#include <nn/os.h>
#include <nn/mem.h>
#include <nn/account.h>
#include <nn/err/err_Api.h>
#include <nn/nn_Macro.h>
#include <nn/nn_Log.h>
#include <nn/nn_Assert.h>
#include <nn/socket/socket_Api.h>
#include <nn/nifm/nifm_ApiNetworkConnection.h>
#include <nn/nifm/nifm_Api.h>
#include <nn/account/account_Api.h>
#include <nn/account/account_Result.h>
#include <nex.h>
#include <curl/curl.h>

//---------------------------------------------------------------------------
//externs junk
//TODO: if you want to share this with other people, we should pass in an opaque pointer to a UserHandle, and prescribe that people shall procure it themselves
//(return (void*)&handle from your nx init code)
namespace Riddlersoft {
	namespace Core {
		namespace Switch {
			extern nn::account::Uid _activeUser;
			extern nn::account::UserHandle handle;
		}
	}
}

//---------------------------------------------------------------------------
//globals and throttle

namespace
{
	unsigned int gameServerId;
	char ngsAccessKey[9];

	int64_t GetTimestamp()
	{
		return nn::os::GetSystemTick().ToTimeSpan().GetMilliSeconds();
	}

	class Throttle
	{
	public:
		enum Category
		{
			PutScore = 0,
			PutCommonData = 0,
			GetRanking = 1
		};

		std::vector<int64_t> entries[2];

		void WaitForSync(Category cat)
		{
			while (!TryFor(cat))
			{
				NN_LOG("Throttling\n");
				nn::os::SleepThread(nn::TimeSpan::FromMilliSeconds(100));
			}
		}

		bool TryFor(Category cat)
		{
			int64_t ts = GetTimestamp();

			//update all timestamps
			for (int c = 0; c < 2; c++) {
				size_t j = 0;
				auto &v = entries[c];
				for (size_t i = 0; i < v.size(); ++i) {
					bool keep = true;
					if (entries[c][i] < ts - 60 * 1000) keep = false;
					if (keep) v[j++] = v[i];
				}
				// trim vector to new size
				v.resize(j);
			}

			const int kLimit = (((int)cat) == 0 ? 10 : 20) - 1; //-1, to fix an off-by-one error that made us fail lotcheck
			if (entries[cat].size() >= kLimit) {
				return false;
			}

			entries[cat].push_back(ts);
			return true;
		}
	} throttle;

	static void ReportQResultReturnCode(const nn::nex::qResult& result, const char* pHeaderText)
	{
		(void)result;
		if (pHeaderText)
		{
			NN_LOG("%s ", pHeaderText);
		}

		NN_LOG("qResult Return Code: 0x%x " "%s" "\n", result.GetReturnCode(), result.GetReturnCodeString());
	}

	void endAsync(bool result)
	{
		//to make sure previous writes to the data structure are complete
		std::atomic_thread_fence(std::memory_order_seq_cst);
		Riddlersoft::Core::Networking::Leaderboards::asyncComm.signal = true;
		Riddlersoft::Core::Networking::Leaderboards::asyncComm.result = result;
	}

	void PrintRanking(const nn::nex::Ranking2Info& rankingInfo)
	{
		NN_LOG("PrintRanking()\n  numRankedIn: %u\n", rankingInfo.GetNumRankedIn());
		NN_LOG("  lowestRank: %u\n", rankingInfo.GetLowestRank());
		NN_LOG("  season: %u\n", rankingInfo.GetSeason());

		nn::nex::qVector<nn::nex::Ranking2RankData> rankDataList = rankingInfo.GetRankDataList();
		nn::nex::qVector<nn::nex::Ranking2RankData>::iterator it;

		for (it = rankDataList.begin(); it != rankDataList.end(); ++it)
		{
			NN_LOG("  Rank: %u\n", it->GetRank());
			NN_LOG("    Score:       %u\n", it->GetScore());
			NN_LOG("    PrincipalId: %llu\n", it->GetPrincipalId());
			NN_LOG("    NexUniqueId: %llu\n", it->GetNexUniqueId());
			NN_LOG("    UserName:    %s", it->GetCommonData().GetUserName().CStr());
		}
	}

	char NN_ALIGNAS(4096) g_SocketMemoryPoolBuffer[nn::socket::DefaultSocketMemoryPoolSize];

} //anonymous namespace

  //---------------------------------------------------------------------------
  //nex session connection status

class NexSession
{
public:
	nn::nex::NgsFacade ngsFacade;
	nn::nex::ProtocolCallContext pccLogin;
	nn::nex::Ranking2Client m_ranking2Client;
	nn::nex::PrincipalID myPrincipalId;

	void TerminateSync()
	{
		nn::nex::ProtocolCallContext oContext;
		ngsFacade.Terminate(&oContext);
		oContext.Wait();
	}

} *session;

//---------------------------------------------------------------------------

static void _init()
{
	NN_ASSERT(nn::socket::Initialize(g_SocketMemoryPoolBuffer,
		nn::socket::DefaultSocketMemoryPoolSize,
		nn::socket::DefaultSocketAllocatorSize,
		nn::socket::DefaultConcurrencyLimit).IsSuccess());

	nn::nifm::Initialize();


	//the demos attempted to connect to the internet here. I dont think that makes sense. I want to go ahead and setup NEX

	// Set an application-defined memory allocator for the NEX library.
	// Allocate a memory block to be used by the NEX library.
	//s_pMemory = new uint8_t[s_workMemorySize];
	//s_demoMem.Initialize(s_pMemory, s_workMemorySize);
	//// Add memory allocation functions to NEX.
	//nn::nex::MemoryManager::SetBasicMemoryFunctions(AllocFunction, FreeFunction);
	nn::nex::MemoryManager::SetBasicMemoryFunctions(malloc, free);

	// (Optionally) set the log level.
	nn::nex::EventLog::GetInstance()->SetLogLevel(nn::nex::EventLog::Debug);
	//nn::nex::EventLog::GetInstance()->SetLogLevel(nn::nex::EventLog::Warning);
	nn::nex::TraceLog::GetInstance()->SetFlag(TRACE_ALWAYS);

	// Set and initialize the thread mode.
	nn::nex::Core::SetThreadMode(nn::nex::Core::ThreadModeInternalTransportBuffer);
	nn::nex::Initialize(true);
}

static void _disconnect()
{
	if (!session) return;
	session->TerminateSync();
	delete session;
	session = nullptr;
}

static bool _connect()
{
	static bool initialized = false;

	if (!initialized)
	{
		initialized = true;
		_init();
	}

	auto ensureResult = nn::account::EnsureNetworkServiceAccountAvailable(Riddlersoft::Core::Switch::handle);
	//user doesnt want to go online, OK
	if (ensureResult <= nn::account::ResultCancelledByUser()) return false;

	//well, looks like it's going to work now.

	//check if we already have a session; check whether its valid
	if (session)
	{
		if (session->ngsFacade.IsConnected())
		{
			//hurrah! no connection needed
			return true;
		}

		//kill and retry
		_disconnect();
	}

	session = new NexSession();

	for (;;)
	{
		NN_LOG("Connecting to the network...\n");

		nn::nifm::SubmitNetworkRequestAndWait();

		//if(nn::nifm::IsNetworkAvailable()) //not needed? HandleNetworkRequestResult deals with it

		// Handles the use request submission results.
		nn::Result result = nn::nifm::HandleNetworkRequestResult();

		if (result.IsSuccess())
		{
			break;
		}
		else if (result <= nn::nifm::ResultErrorHandlingCompleted())
		{
			// Resubmitting the use request may result in it being accepted.
			NN_LOG("Network is not available. SubmitNetworkRequest again.\n");
			continue;
		}
		else
		{
			NN_LOG("Network is not available.\n");
			//nn::err::ShowError(result); //LOTCHECK DOESNT WANT ME TO SHOW THIS ERROR! they granted a temporary waiver on 1st submission
			return false;
		}
	}

	NN_LOG("Network is ready. but more to come\n");

	// Prepare a client for logging in to the game server.
	nn::nex::qResult ret;

	//WARNING: doesnt work if we don't have a nintendo online account (because.. i dont know)
	//"a network service account must be created prior to logging into NGS"
	session->ngsFacade.Login(&session->pccLogin, gameServerId, ngsAccessKey, Riddlersoft::Core::Switch::handle);

	session->pccLogin.Wait();

	if (session->ngsFacade.HasErrorOccuredLastLogin())
	{
		nn::err::ErrorCode errorCode = session->ngsFacade.GetLastLoginErrorCodeStruct();
		NN_LOG("  The login error code is %04u-%04u\n", errorCode.category, errorCode.number);
		nn::err::ShowError(errorCode);
		return false;
	}

	nn::nex::ProtocolCallContext oContext;
	nn::nex::Credentials* pCred = session->ngsFacade.GetCredentials();
	NN_ASSERT(session->m_ranking2Client.Bind(pCred));

	session->myPrincipalId = pCred->GetPrincipalID();

	return true;

BAIL:
	_disconnect();
	return false;
}

static bool _upload_scores_batch(nn::nex::qVector<nn::nex::Ranking2ScoreData> *scores)
{
	throttle.WaitForSync(Throttle::Category::PutScore);

	nn::nex::ProtocolCallContext oContext;
	session->m_ranking2Client.PutScore(&oContext, *scores);
	oContext.Wait();
	if (oContext.GetState() != nn::nex::CallContext::CallSuccess)
	{
		ReportQResultReturnCode(oContext.GetOutcome(), "Failed PutScore");
		nn::err::ShowError(oContext.GetOutcome().GetErrorCodeStruct());
		return false;
	}

	return true;
}

namespace Riddlersoft {
	namespace Core {
		namespace Networking {

			void Leaderboards$_$S__native_init(unsigned int gameServerId, ::System::String* ngsAccessKey)
			{
				::gameServerId = gameServerId;
				for (int i = 0; i < 8; i++) ::ngsAccessKey[i] = (char)String$_get_Chars(ngsAccessKey, i);
				::ngsAccessKey[8] = 0;
			}

			void Leaderboards$_$S__native_uploadScore(::System::ArrayT< ::Riddlersoft::Core::Networking::Leaderboards$Ranking2ScoreUploadRecord*, 1 >* batch, int offset, int count)
			{
				nn::nex::qVector<nn::nex::Ranking2ScoreData> scores;

				if (count == 0)
				{
					Riddlersoft::Core::Networking::Leaderboards::asyncComm.result = false;
					return endAsync(true);
				}

				if (!_connect())
					return endAsync(false);

				for (int i = 0; i < count; i++)
				{
					auto& rec = ArrayT$_Get(batch, offset + i);
					nn::nex::Ranking2ScoreData scoreData;
					scoreData.SetCategory(rec->Category);
					scoreData.SetScore(rec->Score);
					scoreData.SetMisc(rec->Misc);

					scores.push_back(scoreData);

					//break large jobs down into smaller batches
					if (scores.size() == nn::nex::Ranking2Constants::MAX_PUT_MULTI_SCORES)
					{
						if (!_upload_scores_batch(&scores))
							goto BAIL;
						scores.clear();
					}
				}

				if (!_upload_scores_batch(&scores))
					goto BAIL;

			COOL:
				return endAsync(true);

			BAIL:
				_disconnect(); //messed up; connection may be broken, don't leave it open
				return endAsync(false);
			}

			void Leaderboards$_$S__native_uploadCommonData(::System::String* username, ::System::ArrayT< byte_bt, 1 >* binaryData)
			{
				if (!_connect())
					return endAsync(false);

				nn::nex::Ranking2CommonData commonData;

				//can make nex string from char16_bt, which is what we get here
				commonData.SetUserName(nn::nex::String(&username->start_char));

				//set binary data if needed
				if (binaryData)
				{
					nn::nex::qVector<qByte> v;
					//"insert range"
					auto zeroth = &ArrayT$_Get(binaryData, 0);
					auto last = zeroth + binaryData->_firstLength[0];
					v.insert(v.begin(), zeroth, last);
					commonData.SetBinaryData(v);
				}

				throttle.WaitForSync(Throttle::Category::PutCommonData);

				nn::nex::ProtocolCallContext oContext;
				session->m_ranking2Client.PutCommonData(&oContext, commonData);
				oContext.Wait();

				if (oContext.GetState() != nn::nex::CallContext::CallSuccess)
				{
					ReportQResultReturnCode(oContext.GetOutcome(), "Failed to PutCommonData()\n");
					nn::err::ShowError(oContext.GetOutcome().GetErrorCodeStruct());
					goto BAIL;
				}

				return endAsync(true);

			BAIL:
				_disconnect(); //messed up; connection may be broken, don't leave it open
				return endAsync(false);
			}

			void Leaderboards$_$S__native_getScores(unsigned int category, ::Riddlersoft::Core::Networking::LeaderboardFilter filter, unsigned int offset, ::System::Collections::Generic::List$1< ::Riddlersoft::Core::Networking::HighScore >* result)
			{
				nn::nex::ProtocolCallContext oContext;
				nn::nex::Ranking2GetParam getParam;
				nn::nex::Ranking2Info rankingInfo;
				bool didRetry = false;

				if (filter == ::Riddlersoft::Core::Networking::LeaderboardFilter::NEAR_RANKING)
				{
					getParam.SetLength(nn::nex::Ranking2Constants::MAX_RANKING_LENGTH);
					getParam.SetMode(nn::nex::Ranking2Mode::NEAR_RANKING);
				}
				else if (filter == ::Riddlersoft::Core::Networking::LeaderboardFilter::RANGE_RANKING)
				{
					getParam.SetLength(nn::nex::Ranking2Constants::MAX_RANKING_LENGTH);
					getParam.SetMode(nn::nex::Ranking2Mode::RANGE_RANKING);
					getParam.SetOffset(offset);
				}
				else if (filter == ::Riddlersoft::Core::Networking::LeaderboardFilter::FRIEND_RANKING)
				{
					getParam.SetLength(nn::nex::Ranking2Constants::MAX_FRIEND_RANKING_LENGTH);
					getParam.SetMode(nn::nex::Ranking2Mode::FRIEND_RANKING);
				}

				getParam.SetCategory(category);


				//for some reason I tried these twice. Maybe they're more prone to fail.

				if (!_connect())
					return endAsync(false);

			RETRY:
				//getParam.SetOffset(-1); //test of error
				throttle.WaitForSync(Throttle::Category::GetRanking);
				session->m_ranking2Client.GetRanking(&oContext, getParam, &rankingInfo);

				oContext.Wait();

				if (oContext.GetState() != nn::nex::CallContext::CallSuccess)
				{
					ReportQResultReturnCode(oContext.GetOutcome(), "Failed GetRanking");
					switch (oContext.GetOutcome().GetReturnCode())
					{
					case QERROR(RendezVous, ConnectionDisconnected):
						//try reconnecting once before we give up
						if (didRetry)
							goto BAIL;
						_disconnect();
						if (!_connect())
							goto BAIL;
						didRetry = true;
						goto RETRY;

					default:
						//GUESS WHAT! THIS WORKS FROM ANOTHER THREAD! HURRAH!
						nn::err::ShowError(oContext.GetOutcome().GetErrorCodeStruct());
						goto BAIL;
					}
				}

#define HACK_PRINT_RANKING
#ifdef HACK_PRINT_RANKING
				PrintRanking(rankingInfo);
#endif

				{
					nn::nex::qVector<nn::nex::Ranking2RankData> rankDataList = rankingInfo.GetRankDataList();
					nn::nex::qVector<nn::nex::Ranking2RankData>::iterator it;

					int hax = 0;

#ifdef HACK_TEST_LOTSA //this hack is useful for populating your leaderboard with more data during development
					for (int i = 0; i < 30; i++, hax++)
#endif
						for (it = rankDataList.begin(); it != rankDataList.end(); ++it)
						{
							auto LR = ::System::Internal::DefaultValue< ::Riddlersoft::Core::Networking::HighScore >();

							LR->Rank = it->GetRank();
							LR->Score = it->GetScore() + hax;
							LR->Principal_ID = it->GetPrincipalId();
							LR->Misc = it->GetMisc();
							LR->Nex_Unique_ID = it->GetNexUniqueId();

							const auto& commonData = it->GetCommonData();

							//username
							const auto &name = commonData.GetUserName();
							char16_t* wnameptr;
							name.CreateCopy(&wnameptr);
							LR->CommonDataUserName = System::String$_$Ctor(nullptr, wnameptr);
							nn::nex::String::ReleaseCopy(wnameptr);

							//binary data
							auto binaryData = commonData.GetBinaryData();
							LR->CommonDataBinary = ::System::Internal::CreateArray< byte_bt >((int)binaryData.size());
							if (binaryData.size() > 0)
							{
								void* ptr = &ArrayT$_Get(LR->CommonDataBinary, 0);
								memcpy(ptr, &binaryData[0], binaryData.size());
							}

							::System::Collections::Generic::List$1$_Add_Void< ::Riddlersoft::Core::Networking::HighScore >(result, LR);
						}
				}

				return endAsync(true);

			BAIL:
				_disconnect(); //messed up; connection may be broken, don't leave it open
				return endAsync(false);
			}

			unsigned long long Leaderboards$_$S__native_getLastPrincipalId()
			{
				//just in case this is called after failure, return something in valid
				if (!session) return nn::nex::INVALID_UNIQUEID;
				return session->myPrincipalId;
			}

		}
	}
} // namespace Riddlersoft.Core.Networking

