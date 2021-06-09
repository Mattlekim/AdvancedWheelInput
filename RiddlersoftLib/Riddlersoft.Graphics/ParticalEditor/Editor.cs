using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WinForms;
using Riddlersoft.Graphics.Particals;
using Riddlersoft.Graphics.Particals.Emitters;
using Riddlersoft.Graphics.Particals.Modifyers;
using System.IO;

namespace Riddlersoft.Graphics.Particals.ParticalEditor
{
    public class Editor
    {
        private Texture2D _background = null;

        private string _currentFile = "";

        private bool _haveIntialized = false;

        private WinForms.GameScreen _gameScreen;

        private Riddlersoft.Core.Screen.ScreenManiger _screenManiger;

        private ParticalEffect _effect;
        private Emitter _emitter;
        private Form _form;
        private Form _frm_newModifyers;
        private Form _frm_EditModifyer;

        private Form _frm_newEmmiter;

        private Color GetColorForCustomMode(int id)
        {
            int r = Convert.ToInt32(((TextBox)_frm_EditModifyer.FindComponateById($"txt_col_{id}_r")).Text);
            int g = Convert.ToInt32(((TextBox)_frm_EditModifyer.FindComponateById($"txt_col_{id}_g")).Text);
            int b = Convert.ToInt32(((TextBox)_frm_EditModifyer.FindComponateById($"txt_col_{id}_b")).Text);
            int a = Convert.ToInt32(((TextBox)_frm_EditModifyer.FindComponateById($"txt_col_{id}_a")).Text);

            r = MathHelper.Clamp(r, 0, 255);
            g= MathHelper.Clamp(g, 0, 255);
            b = MathHelper.Clamp(b, 0, 255);
            a = MathHelper.Clamp(a, 0, 255);

            return new Color(r, g, b, a);
        }
        private void updateModifyerForm()
        {
            _frm_EditModifyer.Componates.Clear();

            if (_frm_EditModifyer.CustomData is CustomModifyer)
            {
                CustomModifyer cm = _frm_EditModifyer.CustomData as CustomModifyer;
                cm.SetUpEditorProperties();
                int posY = 70;
                int id = 0;
                foreach (CustomModifyer.EditorData ed in cm.EditorProperties)
                {
                    switch (ed.InputType)
                    {
                        case CustomModifyer.EditorInputType.Textbox:
                            _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, posY + 2), _frm_EditModifyer, ed.Name));

                            if (ed.DataType == CustomModifyer.EditorInputDataType.Colour)
                            {
                                _frm_EditModifyer.AddComponate(new WinLable(new Vector2(75, posY + 2), _frm_EditModifyer, "R"));
                                _frm_EditModifyer.AddComponate(new WinLable(new Vector2(125, posY + 2), _frm_EditModifyer, "G"));
                                _frm_EditModifyer.AddComponate(new WinLable(new Vector2(175, posY + 2), _frm_EditModifyer, "B"));
                                _frm_EditModifyer.AddComponate(new WinLable(new Vector2(225, posY + 2), _frm_EditModifyer, "A"));
                                Color col = (Color)ed.Read();
                                TextBox ctb = new TextBox(new Rectangle(90, posY, 30, 20), _frm_EditModifyer)
                                {
                                    InputType = TextBoxInputType.Int,
                                    id = $"txt_col_{id}_r",
                                    CustomData = id,
                                    Text = col.R.ToString(),

                                    OnLeave = (object sender)=>
                                    {
                                        ed.Set(GetColorForCustomMode((int)((TextBox)sender).CustomData));
                                    },
                                };
                                _frm_EditModifyer.AddComponate(ctb);

                                ctb = new TextBox(new Rectangle(140, posY, 30, 20), _frm_EditModifyer)
                                {
                                    InputType = TextBoxInputType.Int,
                                    id = $"txt_col_{id}_g",
                                    CustomData = id,
                                    Text = col.G.ToString(),
                                    OnLeave = (object sender) =>
                                    {
                                        ed.Set(GetColorForCustomMode((int)((TextBox)sender).CustomData));
                                    },
                                };
                                _frm_EditModifyer.AddComponate(ctb);

                                ctb = new TextBox(new Rectangle(190, posY, 30, 20), _frm_EditModifyer)
                                {
                                    InputType = TextBoxInputType.Int,
                                    id = $"txt_col_{id}_b",
                                    CustomData = id,
                                    Text = col.B.ToString(),
                                    OnLeave = (object sender) =>
                                    {
                                        ed.Set(GetColorForCustomMode((int)((TextBox)sender).CustomData));
                                    },
                                };
                                _frm_EditModifyer.AddComponate(ctb);

                                ctb = new TextBox(new Rectangle(240, posY, 30, 20), _frm_EditModifyer)
                                {
                                    InputType = TextBoxInputType.Int,
                                    id = $"txt_col_{id}_a",
                                    CustomData = id,
                                    Text = col.A.ToString(),
                                    OnLeave = (object sender) =>
                                    {
                                        ed.Set(GetColorForCustomMode((int)((TextBox)sender).CustomData));
                                    },
                                };
                                _frm_EditModifyer.AddComponate(ctb);
                            }
                            else
                            {
                                TextBox tb = new TextBox(new Rectangle(120, posY, 150, 20), _frm_EditModifyer);

                                if (ed.DataType == CustomModifyer.EditorInputDataType.Int)
                                {
                                    tb.InputType = TextBoxInputType.Int;
                                    tb.Text = ed.Read().ToString();
                                }
                                else
                                    if (ed.DataType == CustomModifyer.EditorInputDataType.Float)
                                {
                                    tb.InputType = TextBoxInputType.Float;
                                    tb.Text = ed.Read().ToString();
                                }
                                else
                                {
                                    tb.InputType = TextBoxInputType.Full;

                                    if (ed.DataType == CustomModifyer.EditorInputDataType.String)
                                        tb.Text = (string)ed.Read();
                                    if (ed.DataType == CustomModifyer.EditorInputDataType.Vector2)
                                    {
                                        Vector2 v = (Vector2)ed.Read();
                                        tb.Text = $"{v.X}, {v.Y}";
                                    }



                                }

                                tb.OnLeave = (object sender) =>
                                {
                                    ed.Set(tb.Text);
                                };

                                _frm_EditModifyer.AddComponate(tb);
                            }
                            break;

                        case CustomModifyer.EditorInputType.TickBox:
                            TickBox t = new TickBox(new Rectangle(120, posY, 20,20), _frm_EditModifyer);

                            if (ed.DataType == CustomModifyer.EditorInputDataType.Bool)
                            {
                                t.Text = ed.Name;
                                t.TextOffset = new Vector2(0, 10);
                                t.Ticked = (bool)ed.Read();

                                t.OnChange = (object sender) =>
                                {
                                    TickBox tt = sender as TickBox;
                                    ed.Set(tt.Ticked);
                                };
                            }
                            _frm_EditModifyer.AddComponate(t);
                            break;

                    }
                    posY += 30;
                }

            }
            else
            {
                if (_frm_EditModifyer.CustomData is DampaningModifyer)
                {
                    #region dampaning modifyer
                    DampaningModifyer dm = (DampaningModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 70), _frm_EditModifyer, "Damping Factor"));
                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 130), _frm_EditModifyer, "Anglulor Damping"));
                    _frm_EditModifyer.AddComponate(new TickBox(new Rectangle(150, 40, 20, 20), _frm_EditModifyer)
                    {
                        TextOffset = new Vector2(0, 8),
                        Text = "Dampen Velocity",
                        Ticked = dm.DampenVelocity,

                        OnChange = (object sender) =>
                        {
                            TickBox tb = sender as TickBox;
                            dm.DampenVelocity = tb.Ticked;
                        }
                    });

                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 70, 100, 20), _frm_EditModifyer)
                    {
                        Text = dm.DampaningAmount.ToString(),
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            dm.DampaningAmount = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }

                    });

                    _frm_EditModifyer.AddComponate(new TickBox(new Rectangle(150, 100, 20, 20), _frm_EditModifyer)
                    {
                        TextOffset = new Vector2(0, 8),
                        Text = "Dampen Rotation",
                        Ticked = dm.DampenAngulorVelocity,

                        OnChange = (object sender) =>
                        {
                            TickBox tb = sender as TickBox;
                            dm.DampenAngulorVelocity = tb.Ticked;
                        }
                    });

                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 130, 100, 20), _frm_EditModifyer)
                    {
                        Text = dm.AngulorDampaningAmount.ToString(),
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            dm.AngulorDampaningAmount = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is LinearColourModifyer)
                {
                    #region linear color
                    LinearColourModifyer lcm = (LinearColourModifyer)_frm_EditModifyer.CustomData;
                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 42), _frm_EditModifyer, "Start Colour"));
                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 70), _frm_EditModifyer, "End Colour"));

                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 40, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_s_r",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.InitialColor.R.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.InitialColor = new Color((byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.InitialColor.G, lcm.InitialColor.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(160, 40, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_s_g",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.InitialColor.G.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.InitialColor = new Color(lcm.InitialColor.R, (byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.InitialColor.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(220, 40, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_s_b",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.InitialColor.B.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.InitialColor = new Color(lcm.InitialColor.R, lcm.InitialColor.G, (byte)SetValueFromTextbox((TextBox)sender, 0, 255));
                        }
                    });

                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 70, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_e_r",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.EndColor.R.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.EndColor = new Color((byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.EndColor.G, lcm.EndColor.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(160, 70, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_e_g",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.EndColor.G.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.EndColor = new Color(lcm.EndColor.R, (byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.EndColor.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(220, 70, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_e_b",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.EndColor.B.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.EndColor = new Color(lcm.EndColor.R, lcm.EndColor.G, (byte)SetValueFromTextbox((TextBox)sender, 0, 255));
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is LinearGravityModifyer)
                {
                    #region gravity
                    LinearGravityModifyer lgm = (LinearGravityModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 40), _frm_EditModifyer, "Gravity"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 40, 100, 20), _frm_EditModifyer)
                    {
                        Text = $"{lgm.Gravity.X}, {lgm.Gravity.Y}",
                        id = "txt_gravity",
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            lgm.Gravity = SetVector2FromTextbox((TextBox)sender, -100f, 100f);
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is LinearScaleModifyer)
                {
                    #region linear scale
                    LinearScaleModifyer lgm = (LinearScaleModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 40), _frm_EditModifyer, "End Scale Multiplyer"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 40, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.EndScale.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.EndScale = SetValueFromTextbox((TextBox)sender, 0f, 100f);
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is LinearFadeModifyer)
                {
                    #region linear fade
                    LinearFadeModifyer lgm = (LinearFadeModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 40), _frm_EditModifyer, "Start Fade"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 40, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.InitialFade.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.InitialFade = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 70), _frm_EditModifyer, "End Fade"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 70, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.EndFade.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.EndFade = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is OsolatingModifyer)
                {
                    #region osolating
                    OsolatingModifyer lgm = (OsolatingModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 42), _frm_EditModifyer, "Deviation Amount:"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 40, 100, 20), _frm_EditModifyer)
                    {
                        Text = lgm.Amount.ToString(),
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            lgm.Amount = SetValueFromTextbox((TextBox)sender, -1000, 1000);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 72), _frm_EditModifyer, "Speed:"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 70, 100, 20), _frm_EditModifyer)
                    {
                        Text = lgm.Speed.ToString(),
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            lgm.Speed = SetValueFromTextbox((TextBox)sender, 0, 100);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new TickBox(new Rectangle(150, 100, 20, 20), _frm_EditModifyer)
                    {
                        Ticked = lgm.XAxis,
                        Text = "Osolate X Axis",
                        TextOffset = new Vector2(0, 10),
                        OnLeftClick = (object sender, Point pos) =>
                        {
                            lgm.XAxis = ((TickBox)sender).Ticked;
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TickBox(new Rectangle(150, 130, 20, 20), _frm_EditModifyer)
                    {
                        Ticked = lgm.YAxis,
                        Text = "Osolate Y Axis",
                        TextOffset = new Vector2(0, 10),
                        OnLeftClick = (object sender, Point pos) =>
                        {
                            lgm.YAxis = ((TickBox)sender).Ticked;
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is RotatingOsolatingModifyer)
                {
                    #region rotating osolating
                    RotatingOsolatingModifyer lgm = (RotatingOsolatingModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 42), _frm_EditModifyer, "Deviation Amount:"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 40, 100, 20), _frm_EditModifyer)
                    {
                        Text = lgm.Amount.ToString(),
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            lgm.Amount = SetValueFromTextbox((TextBox)sender, -1000, 1000);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 72), _frm_EditModifyer, "Speed:"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(150, 70, 100, 20), _frm_EditModifyer)
                    {
                        Text = lgm.Speed.ToString(),
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                            lgm.Speed = SetValueFromTextbox((TextBox)sender, 0, 100);
                        }
                    });

                    #endregion
                }

                if (_frm_EditModifyer.CustomData is StateColourModifyer)
                {
                    #region state colour
                    StateColourModifyer lcm = (StateColourModifyer)_frm_EditModifyer.CustomData;
                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 42), _frm_EditModifyer, "Start Colour"));
                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 72), _frm_EditModifyer, "Middle Colour"));
                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 102), _frm_EditModifyer, "End Colour"));

                    //start color
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 40, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_s_r",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.InitialColor.R.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.InitialColor = new Color((byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.InitialColor.G, lcm.InitialColor.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(160, 40, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_s_g",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.InitialColor.G.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.InitialColor = new Color(lcm.InitialColor.R, (byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.InitialColor.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(220, 40, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_s_b",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.InitialColor.B.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.InitialColor = new Color(lcm.InitialColor.R, lcm.InitialColor.G, (byte)SetValueFromTextbox((TextBox)sender, 0, 255));
                        }
                    });


                    //=======================middle color==============================
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 70, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_m_r",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.MiddleColour.R.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.MiddleColour = new Color((byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.MiddleColour.G, lcm.MiddleColour.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(160, 70, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_m_g",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.MiddleColour.G.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.MiddleColour = new Color(lcm.MiddleColour.R, (byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.MiddleColour.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(220, 70, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_m_b",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.MiddleColour.B.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.MiddleColour = new Color(lcm.MiddleColour.R, lcm.MiddleColour.G, (byte)SetValueFromTextbox((TextBox)sender, 0, 255));
                        }
                    });

                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 100, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_e_r",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.EndColour.R.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.EndColour = new Color((byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.EndColour.G, lcm.EndColour.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(160, 100, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_e_g",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.EndColour.G.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.EndColour = new Color(lcm.EndColour.R, (byte)SetValueFromTextbox((TextBox)sender, 0, 255), lcm.EndColour.B);
                        }
                    });
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(220, 100, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_e_b",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.EndColour.B.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.EndColour = new Color(lcm.EndColour.R, lcm.EndColour.G, (byte)SetValueFromTextbox((TextBox)sender, 0, 255));
                        }
                    });

                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(220, 130, 50, 20), _frm_EditModifyer)
                    {
                        id = "txt_middle",
                        InputType = TextBoxInputType.Float,
                        Text = lcm.MiddleColourPosition.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lcm.MiddleColourPosition = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });
                    #endregion
                }

                if (_frm_EditModifyer.CustomData is StateFadeModifyer)
                {
                    #region state fade
                    StateFadeModifyer lgm = (StateFadeModifyer)_frm_EditModifyer.CustomData;

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 40), _frm_EditModifyer, "Start Fade"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 40, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.InitialFade.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.InitialFade = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 70), _frm_EditModifyer, "Middle Fade"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 70, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.MiddleStateFade.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.MiddleStateFade = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 100), _frm_EditModifyer, "Middle Fade Lifetime"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 100, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.MiddleLifeTime.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.MiddleLifeTime = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });

                    _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 130), _frm_EditModifyer, "End Fade"));
                    _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(170, 130, 100, 20), _frm_EditModifyer)
                    {
                        InputType = TextBoxInputType.Float,
                        Text = lgm.EndFade.ToString(),
                        OnLeave = (object sender) =>
                        {
                            lgm.EndFade = SetValueFromTextbox((TextBox)sender, 0f, 1f);
                        }
                    });
                    #endregion
                }
            }
            _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 250), _frm_EditModifyer, "Start Time"));
            _frm_EditModifyer.AddComponate(new WinLable(new Vector2(10, 280), _frm_EditModifyer, "End Time"));

            Modifyer mod = _frm_EditModifyer.CustomData as Modifyer;

            _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 245, 100, 20), _frm_EditModifyer)
            {
                Text = mod.StartTime.ToString(),
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender)=>
                {
                    TextBox tb = sender as TextBox;
                    mod.StartTime = SetValueFromTextbox(tb, 0f, 1000f);
                }
            });
            _frm_EditModifyer.AddComponate(new TextBox(new Rectangle(100, 275, 100, 20), _frm_EditModifyer)
            {
                Text = mod.EndTime.ToString(),
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    TextBox tb = sender as TextBox;
                    mod.EndTime = SetValueFromTextbox(tb, 0f, 1000f);
                }
            });
            _frm_EditModifyer.AddComponate(new TickBox(new Rectangle(100, 305, 20, 20), _frm_EditModifyer)
            {
                Text = "Percentage Based Time",
                Ticked = mod.UsePercentageFromTime,
                OnChange = (object sender)=>
                {
                    mod.UsePercentageFromTime = ((TickBox)sender).Ticked;
                }
            
            });
        }

        private void UpdateForm()
        {
            _form.Componates.Clear();
            _form.AddComponate(new WinLable(new Vector2(10, 50), _form, "Emmiters"));
            _form.AddComponate(new Button(new Rectangle(100, 50, 20, 20), _gameScreen, "+")
            {
                OnLeftClick = (object sender, Point pos) =>
                {
                    _gameScreen.ShowComponate("frm_newemmiter", null, true);


                },
            });



            int startY = 80;
            if (_effect != null)
            {
                int index = 0;
                foreach (Emitter e in _effect._emitters)
                {
                    _form.AddComponate(new WinLable(new Vector2(50, startY), _form, e.Name)
                    {
                        OnLeftClick = (object sender, Point pos) =>
                        {
                            WinLable wl = sender as WinLable;
                            foreach (Emitter em in _effect.Emitters)
                                if (em.Name == wl.Text) //look for the emmiter
                                {
                                    _emitter = em; //copy the emmiter to the tmp memory
                                    break;
                                }

                            UpdateForm();
                        }
                    });

                    _form.AddComponate(new Button(new Rectangle(140, startY, 50, 20), _form, "Delete")
                    {
                        TextColourPrimary = Color.Red,
                        CustomData = index,
                        OnLeftClick = (object sender, Point pos) =>
                        {
                            _emitter = null;
                            _effect.Emitters.RemoveAt((int)((Componate)sender).CustomData);
                            UpdateForm();
                        }
                    });
                    startY += 25;
                    index++;
                }
            }
            _form.AddComponate(new Divider(startY, _form));
            startY += 15;

            _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Name: "));
            _form.AddComponate(new TextBox(new Rectangle(60, startY, 130, 20), _form)
            {
                id = "txt_name",
                Active = false,
                OnLeave = (object sender) =>
                {
                    if (_emitter == null)
                        return;
                    TextBox Tb = sender as TextBox;
                    _emitter.Name = Tb.Text;

                }
            });


            bool haveRadius = true;

            if (_emitter != null)
            {
                if (_emitter is RectangleEmitter)
                    haveRadius = false;
            }

            startY += 20;
            if (haveRadius)
            {
                _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Radius: "));
                _form.AddComponate(new TextBox(new Rectangle(140, startY, 50, 20), _form)
                {
                    id = "txt_radius",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                    OnLeave = (object sender) =>
                    {
                        if (_emitter is CircleEmitter)
                            ((CircleEmitter)_emitter).Radius = SetValueFromTextbox((TextBox)sender, 1, 1000);


                        if (_emitter is ConeEmitter)
                            ((ConeEmitter)_emitter).Radius = SetValueFromTextbox((TextBox)sender, 1, 1000);
                    }
                });

                startY += 20;
                _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Offset: "));
                _form.AddComponate(new TextBox(new Rectangle(80, startY, 110, 20), _form)
                {
                    id = "txt_offset",
                    Active = false,
                    InputType = TextBoxInputType.Full,
                    OnLeave = (object sender) =>
                    {
                        if (_emitter is CircleEmitter)
                            ((CircleEmitter)_emitter).Offset = SetVector2FromTextbox((TextBox)sender, -1000, 1000);


                        if (_emitter is ConeEmitter)
                            ((ConeEmitter)_emitter).Offset = SetVector2FromTextbox((TextBox)sender, -1000, 1000);
                    }
                });
                if (_emitter is ConeEmitter)
                {
                    startY += 20;

                    _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Angle: "));
                    _form.AddComponate(new TextBox(new Rectangle(80, startY, 50, 20), _form)
                    {
                        id = "txt_angle_start",
                        Active = false,
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {
                          
                            if (_emitter is ConeEmitter)
                                ((ConeEmitter)_emitter).MinRadious = SetValueFromTextbox((TextBox)sender, -10f, 10f);
                        }
                    });

                    _form.AddComponate(new TextBox(new Rectangle(140, startY, 50, 20), _form)
                    {
                        id = "txt_angle_end",
                        Active = false,
                        InputType = TextBoxInputType.Float,
                        OnLeave = (object sender) =>
                        {

                            if (_emitter is ConeEmitter)
                                ((ConeEmitter)_emitter).MaxRadious = SetValueFromTextbox((TextBox)sender, -10f, 10f);
                        }
                    });
                }
            }
            else
            {
                _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Size:"));
                _form.AddComponate(new TextBox(new Rectangle(50, startY, 30, 20), _form)
                {
                    id = "txt_size_x",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                    OnLeave = (object sender) =>
                    {
                        Rectangle r = ((RectangleEmitter)_emitter).Area;
                        r.X = SetValueFromTextbox((TextBox)sender, -99999, 99999);
                        ((RectangleEmitter)_emitter).SetArea(r);
                    }
                });

                _form.AddComponate(new TextBox(new Rectangle(80, startY, 30, 20), _form)
                {
                    id = "txt_size_y",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                       
                    OnLeave = (object sender) =>
                    {
                        Rectangle r = ((RectangleEmitter)_emitter).Area;
                        r.Y = SetValueFromTextbox((TextBox)sender, -99999, 99999);
                        ((RectangleEmitter)_emitter).SetArea(r);
                    }
                });
                
                _form.AddComponate(new TextBox(new Rectangle(110, startY, 40, 20), _form)
                {
                    id = "txt_size_w",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                    OnLeave = (object sender) =>
                    {
                        Rectangle r = ((RectangleEmitter)_emitter).Area;
                        r.Width = SetValueFromTextbox((TextBox)sender, -99999, 99999);
                        ((RectangleEmitter)_emitter).SetArea(r);
                    }
                });

                _form.AddComponate(new TextBox(new Rectangle(150, startY, 40, 20), _form)
                {
                    id = "txt_size_h",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                    OnLeave = (object sender) =>
                    {
                        Rectangle r = ((RectangleEmitter)_emitter).Area;
                        r.Height = SetValueFromTextbox((TextBox)sender, -99999, 99999);
                        ((RectangleEmitter)_emitter).SetArea(r);
                    }
                });
            }

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Max Particals:"));
            _form.AddComponate(new TextBox(new Rectangle(110, startY, 80, 20), _form)
            {
                id = "txt_maxparticals",
                Active = false,
                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    _emitter.MaxParticles = SetValueFromTextbox((TextBox)sender, 1, 5000);
                }
            });

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Release Amount: "));
            _form.AddComponate(new TextBox(new Rectangle(140, startY, 50, 20), _form)
            {
                id = "txt_releasequanity",
                Active = false,
                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    TextBox Tb = sender as TextBox;
                    if (Tb.Text == "0")
                        Tb.Text = "1";
                    if (Tb.Text.Length > 0)
                        _emitter.ReleaseAmount = (float)Convert.ToDouble(Tb.Text);

                }
            });

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "LifeTime: "));
            _form.AddComponate(new TextBox(new Rectangle(80, startY, 50, 20), _form)
            {
                id = "txt_lifetime_start",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    TextBox Tb = sender as TextBox;
                    if (Tb.Text.Length > 0)
                    {
                        _emitter.Initial_LifeTime = new FloatRange((float)Convert.ToDouble(Tb.Text), _emitter.Initial_LifeTime.Two);
                        if (_emitter.Initial_LifeTime.One < 0)
                            _emitter.Initial_LifeTime = new FloatRange(0, _emitter.Initial_LifeTime.Two);


                    }


                }
            });

            _form.AddComponate(new TextBox(new Rectangle(140, startY, 50, 20), _form)
            {
                id = "txt_lifetime_end",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    TextBox Tb = sender as TextBox;
                    if (Tb.Text.Length > 0)
                    {
                        _emitter.Initial_LifeTime = new FloatRange(_emitter.Initial_LifeTime.One, (float)Convert.ToDouble(Tb.Text));
                        if (_emitter.Initial_LifeTime.Two < 0)
                            _emitter.Initial_LifeTime = new FloatRange(_emitter.Initial_LifeTime.One, 0);
                    }


                }
            });

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Col 1: "));

            _form.AddComponate(new WinLable(new Vector2(60, startY + 2), _form, "R: "));
            _form.AddComponate(new WinLable(new Vector2(100, startY + 2), _form, "G: "));
            _form.AddComponate(new WinLable(new Vector2(150, startY + 2), _form, "B: "));
            _form.AddComponate(new TextBox(new Rectangle(70, startY, 30, 20), _form)
            {
                id = "txt_c1r",
                Active = false,
                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    Color col = _emitter.Initial_Colour.One;
                    col.R = (byte)SetValueFromTextbox((TextBox)sender, 0, 255);
                    ColourRange cr = _emitter.Initial_Colour;
                    cr.ReplaceOne(col);
                    _emitter.Initial_Colour = cr;
                }
            });

            _form.AddComponate(new TextBox(new Rectangle(115, startY, 30, 20), _form)
            {
                id = "txt_c1g",
                Active = false,
                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    Color col = _emitter.Initial_Colour.One;
                    col.G = (byte)SetValueFromTextbox((TextBox)sender, 0, 255);
                    ColourRange cr = _emitter.Initial_Colour;
                    cr.ReplaceOne(col);
                    _emitter.Initial_Colour = cr;

                }
            });

            _form.AddComponate(new TextBox(new Rectangle(160, startY, 30, 20), _form)
            {
                id = "txt_c1b",
                Active = false,

                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    Color col = _emitter.Initial_Colour.One;
                    col.B = (byte)SetValueFromTextbox((TextBox)sender, 0, 255);
                    ColourRange cr = _emitter.Initial_Colour;
                    cr.ReplaceOne(col);
                    _emitter.Initial_Colour = cr;
                }
            });

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Col 2: "));

            _form.AddComponate(new WinLable(new Vector2(60, startY + 2), _form, "R: "));
            _form.AddComponate(new WinLable(new Vector2(100, startY + 2), _form, "G: "));
            _form.AddComponate(new WinLable(new Vector2(150, startY + 2), _form, "B: "));
            _form.AddComponate(new TextBox(new Rectangle(70, startY, 30, 20), _form)
            {
                id = "txt_c2r",
                Active = false,
                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    Color col = _emitter.Initial_Colour.Two;
                    col.R = (byte)SetValueFromTextbox((TextBox)sender, 0, 255);
                    ColourRange cr = _emitter.Initial_Colour;
                    cr.ReplaceTwo(col);
                    _emitter.Initial_Colour = cr;
                }
            });

            _form.AddComponate(new TextBox(new Rectangle(115, startY, 30, 20), _form)
            {
                id = "txt_c2g",
                Active = false,
                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    Color col = _emitter.Initial_Colour.Two;
                    col.G = (byte)SetValueFromTextbox((TextBox)sender, 0, 255);
                    ColourRange cr = _emitter.Initial_Colour;
                    cr.ReplaceTwo(col);
                    _emitter.Initial_Colour = cr;

                }
            });

            _form.AddComponate(new TextBox(new Rectangle(160, startY, 30, 20), _form)
            {
                id = "txt_c2b",
                Active = false,

                InputType = TextBoxInputType.Int,
                OnLeave = (object sender) =>
                {
                    Color col = _emitter.Initial_Colour.Two;
                    col.B = (byte)SetValueFromTextbox((TextBox)sender, 0, 255);
                    ColourRange cr = _emitter.Initial_Colour;
                    cr.ReplaceTwo(col);
                    _emitter.Initial_Colour = cr;
                }
            });


            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Scale "));

            _form.AddComponate(new WinLable(new Vector2(60, startY + 2), _form, "One: "));
            _form.AddComponate(new WinLable(new Vector2(125, startY + 2), _form, "Two: "));
            _form.AddComponate(new TextBox(new Rectangle(95, startY, 30, 20), _form)
            {
                id = "txt_sOne",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    _emitter.Initial_Scale = SetFloatRangeFromTextbox((TextBox)sender, 0.01f, 100f, _emitter.Initial_Scale, true);
                }
            });

            _form.AddComponate(new TextBox(new Rectangle(160, startY, 30, 20), _form)
            {
                id = "txt_sTwo",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    _emitter.Initial_Scale = SetFloatRangeFromTextbox((TextBox)sender, 0.01f, 100f, _emitter.Initial_Scale, false);
                }
            });


            int dropDownLocation = 0;
            if (_emitter is ConeEmitter|| _emitter is CircleEmitter)
            {
                startY += 20;
                dropDownLocation = startY;
                
            }

            bool have2Velocitys = false;
            if (_emitter is RectangleEmitter)
                have2Velocitys = true;
            else
            {
                if (_emitter is CircleEmitter)
                    if (((CircleEmitter)_emitter).Function == EmmiterFunction.Nutral)
                        have2Velocitys = true;

            }

            if (have2Velocitys)
            {
                startY += 20;
                _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Velocity One"));
                _form.AddComponate(new TextBox(new Rectangle(100, startY, 90, 20), _form)
                {
                    id = "txt_vOne",
                    Active = false,
                    InputType = TextBoxInputType.Full,
                    OnLeave = (object sender) =>
                    {
                        _emitter.Initial_Velocity = SetVector2RangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Velocity, true);
                    }
                });

                startY += 20;
                _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Velocity Two"));
                _form.AddComponate(new TextBox(new Rectangle(100, startY, 90, 20), _form)
                {
                    id = "txt_vTwo",
                    Active = false,
                    InputType = TextBoxInputType.Full,
                    OnLeave = (object sender) =>
                    {
                        _emitter.Initial_Velocity = SetVector2RangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Velocity, false);
                    }
                });
            }
            else
            {
                startY += 20;
                _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Velocity"));
                //_form.AddComponate(new WinLable(new Vector2(70, startY + 2), _form, "X"));
                //_form.AddComponate(new WinLable(new Vector2(135, startY + 2), _form, "Y"));
                _form.AddComponate(new TextBox(new Rectangle(85, startY, 40, 20), _form)
                {
                    id = "txt_speed_one",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                    OnLeave = (object sender) =>
                    {
                        if (_emitter is CircleEmitter)
                            ((CircleEmitter)_emitter).ParticalSpeed = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, ((CircleEmitter)_emitter).ParticalSpeed, true);
                        if (_emitter is ConeEmitter)
                            ((ConeEmitter)_emitter).ParticalSpeed = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, ((ConeEmitter)_emitter).ParticalSpeed, true);
                    }
                });

                _form.AddComponate(new TextBox(new Rectangle(150, startY, 40, 20), _form)
                {
                    id = "txt_speed_two",
                    Active = false,
                    InputType = TextBoxInputType.Float,
                    OnLeave = (object sender) =>
                    {
                        if (_emitter is CircleEmitter)
                            ((CircleEmitter)_emitter).ParticalSpeed = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, ((CircleEmitter)_emitter).ParticalSpeed, false);
                        if (_emitter is ConeEmitter)
                            ((ConeEmitter)_emitter).ParticalSpeed = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, ((ConeEmitter)_emitter).ParticalSpeed, false);
                    }
                });
            }

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Rotation Vel "));
            _form.AddComponate(new TextBox(new Rectangle(100, startY, 40, 20), _form)
            {
                id = "txt_rotvelone",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    _emitter.Initial_Angulor_Velocity = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Angulor_Velocity, true);
                }
            });
            _form.AddComponate(new TextBox(new Rectangle(150, startY, 40, 20), _form)
            {
                id = "txt_rotveltwo",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    _emitter.Initial_Angulor_Velocity = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Angulor_Velocity, false);
                }
            });

            startY += 20;
            _form.AddComponate(new WinLable(new Vector2(10, startY + 2), _form, "Rotation"));
            _form.AddComponate(new TextBox(new Rectangle(100, startY, 40, 20), _form)
            {
                id = "txt_rotone",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    _emitter.Initial_Rotaiton = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Rotaiton, true);
                }
            });
            _form.AddComponate(new TextBox(new Rectangle(150, startY, 40, 20), _form)
            {
                id = "txt_rottwo",
                Active = false,
                InputType = TextBoxInputType.Float,
                OnLeave = (object sender) =>
                {
                    _emitter.Initial_Rotaiton = SetFloatRangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Rotaiton, false);
                }
            });

            startY += 30;
            _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Texture"));
            _form.AddComponate(new PictureBox(new Rectangle(70, startY, 50, 50), _form, _dot, null)
            {
                Active = false,
                id = "pb_texture",
                OnLeftClick = (object sender, Point pos) =>
                {
                    _dialogFilePicker.OnFileSelected = OnFileSelected;
                    _dialogFilePicker.AllowedFileExtentions.Clear();
                    _dialogFilePicker.AllowedFileExtentions.Add("*.png");
                    _dialogFilePicker.ShowLoadDialog(true, string.Empty);
                }
            });

            if (dropDownLocation != 0)
            {
                _form.AddComponate(new WinLable(new Vector2(10, dropDownLocation + 2), _form, "Function"));
                _form.AddComponate(new ListBox(new Rectangle(100, dropDownLocation, 90, 20), _form, new List<string>() { "Nutral", "Explosive", "Implosive" }, true)
                {
                    id = "lb_function",
                    Active = false,

                    OnItemClick = (string id, int index) =>
                    {
                       
                        if (_emitter is CircleEmitter)
                            ((CircleEmitter)_emitter).Function = (EmmiterFunction)index;
                        if (_emitter is ConeEmitter)
                            ((ConeEmitter)_emitter).Function = (EmmiterFunction)index;
                        UpdateForm();
                    // _emitter.Initial_Velocity = SetVector2RangeFromTextbox((TextBox)sender, -100f, 100f, _emitter.Initial_Velocity, true);
                }
                });


                int f = 0;
                if (_emitter is CircleEmitter)
                {
                    f = (int)((CircleEmitter)_emitter).Function;
                }

                if (_emitter is ConeEmitter)
                {
                    f = (int)((ConeEmitter)_emitter).Function;
                }

                SetIndexToListBox("lb_function", f);
            }
            //now we have setup the form we need to set the data

            if (_emitter != null)
            {
                TextBox txt_tmp = _form.FindComponateById("txt_name") as TextBox;
                txt_tmp.Active = true;
                txt_tmp.Text = _emitter.Name;



                txt_tmp = _form.FindComponateById("txt_releasequanity") as TextBox;
                txt_tmp.Active = true;
                txt_tmp.Text = _emitter.ReleaseAmount.ToString();

                txt_tmp = _form.FindComponateById("txt_lifetime_start") as TextBox;
                txt_tmp.Active = true;
                txt_tmp.Text = _emitter.Initial_LifeTime.One.ToString();
                txt_tmp = _form.FindComponateById("txt_lifetime_end") as TextBox;
                txt_tmp.Active = true;
                txt_tmp.Text = _emitter.Initial_LifeTime.Two.ToString();

                if (_emitter is CircleEmitter)
                    SetValueToTextBox("txt_radius", ((CircleEmitter)_emitter).Radius);
                if (_emitter is ConeEmitter)
                    SetValueToTextBox("txt_radius", ((ConeEmitter)_emitter).Radius);

                PictureBox pb = _form.FindComponateById("pb_texture") as PictureBox;
                pb.Activate();
                pb.Img = _emitter.ParticalTexture;

                SetValueToTextBox("txt_c1r", _emitter.Initial_Colour.One.R);
                SetValueToTextBox("txt_c1g", _emitter.Initial_Colour.One.G);
                SetValueToTextBox("txt_c1b", _emitter.Initial_Colour.One.B);

                SetValueToTextBox("txt_c2r", _emitter.Initial_Colour.Two.R);
                SetValueToTextBox("txt_c2g", _emitter.Initial_Colour.Two.G);
                SetValueToTextBox("txt_c2b", _emitter.Initial_Colour.Two.B);

                SetValueToTextBox("txt_sOne", _emitter.Initial_Scale.One);
                SetValueToTextBox("txt_sTwo", _emitter.Initial_Scale.Two);

                SetValueToTextBox("txt_rotvelone", _emitter.Initial_Angulor_Velocity.One);
                SetValueToTextBox("txt_rotveltwo", _emitter.Initial_Angulor_Velocity.Two);

                SetValueToTextBox("txt_rotone", _emitter.Initial_Rotaiton.One);
                SetValueToTextBox("txt_rottwo", _emitter.Initial_Rotaiton.Two);

                SetValueToTextBox("txt_maxparticals", _emitter.MaxParticles);

                if (haveRadius)
                {
                    SetValueToTextBox("txt_offset", ((CircleEmitter)_emitter).Offset);
                }

                if (_emitter is ConeEmitter)
                {
                    SetValueToTextBox("txt_angle_start", (float)Math.Round(((ConeEmitter)_emitter).MinRadious,3));
                    SetValueToTextBox("txt_angle_end", (float)Math.Round(((ConeEmitter)_emitter).MaxRadious,3));
                }

                if (_emitter is RectangleEmitter)
                {
                    SetValueToTextBox("txt_size_x", ((RectangleEmitter)_emitter).Area.X);
                    SetValueToTextBox("txt_size_y", ((RectangleEmitter)_emitter).Area.Y);
                    SetValueToTextBox("txt_size_w", ((RectangleEmitter)_emitter).Area.Width);
                    SetValueToTextBox("txt_size_h", ((RectangleEmitter)_emitter).Area.Height);
                }

                if (have2Velocitys)
                {
                    SetValueToTextBox("txt_vOne", _emitter.Initial_Velocity.One);
                    SetValueToTextBox("txt_vTwo", _emitter.Initial_Velocity.Two);
                }
                else
                {
                    int f = 0;
                    if (_emitter is CircleEmitter)
                    {
                        SetValueToTextBox("txt_speed_one", ((CircleEmitter)_emitter).ParticalSpeed.One);
                        SetValueToTextBox("txt_speed_two", ((CircleEmitter)_emitter).ParticalSpeed.Two);
                        f = (int)((CircleEmitter)_emitter).Function;
                    }

                    if (_emitter is ConeEmitter)
                    {
                        SetValueToTextBox("txt_speed_one", ((ConeEmitter)_emitter).ParticalSpeed.One);
                        SetValueToTextBox("txt_speed_two", ((ConeEmitter)_emitter).ParticalSpeed.Two);
                        f = (int)((ConeEmitter)_emitter).Function;
                    }

                   
                }
            }
            
            startY += 60;
            _form.AddComponate(new Divider(startY, _form));
            startY += 10;
            _form.AddComponate(new WinLable(new Vector2(10, startY), _form, "Modifyers"));
            _form.AddComponate(new Button(new Rectangle(100, startY, 20, 20), _form, "+")
            {
                Active = false,
                id = "bnt_add_modifyer",
                OnLeftClick = (object sender, Point pos) =>
                {
                    Controler.ShowComponate("frm_add_moddifyer", null, true);
                    
                }
            });

        if (_emitter != null)
            {
                _form.FindComponateById("bnt_add_modifyer").Activate();
                int i = 0;
                foreach(Modifyer m in _emitter.Modifyers)
                {
                    startY += 20;
                    string tmp = m.GetType().ToString();
                    int index = tmp.LastIndexOf(".");
                    tmp = tmp.Substring(index + 1);
                    _form.AddComponate(new WinLable(new Vector2(10, startY), _form, tmp)
                    {
                        CustomData = i,
                        OnLeftClick = (object sender, Point pos) =>
                        {
                            _frm_EditModifyer.CustomData = (Modifyer)_emitter.Modifyers[(int)((Componate)sender).CustomData];
                            _frm_EditModifyer.Activate(false);
                            updateModifyerForm();
                        }
                    });

                    _form.AddComponate(new Button(new Rectangle(140, startY, 50, 20), _form, "Delete")
                    {
                        TextColourPrimary = Color.Red,
                        CustomData = i,
                        OnLeftClick = (object sender, Point pos) =>
                        {
                            _emitter.Modifyers.RemoveAt((int)((Componate)sender).CustomData);
                            UpdateForm();
                        }
                    });

                    i++;
                }
            }
            return;

            _form.AddComponate(new Button(new Rectangle(10, 680, 60, 30), _form, "Save")
            {
                OnLeftClick = (object sender, Point pos)=>
                {
                    if (_currentFile != string.Empty)
                    {
                        using (FileStream stream = File.Create(_currentFile))
                            _effect.SaveToStream(stream);
                        return;
                    }
                    _dialogSavingLoading.AllowedFileExtentions.Clear();
                    _dialogSavingLoading.AllowedFileExtentions.Add(".pe");
                    _dialogSavingLoading.ShowSaveDialog(true,_currentFile);


                    _dialogSavingLoading.OnFileSelected = (string path, FileDialogType type) =>
                    {
                        using (FileStream stream = File.Create(path))
                            _effect.SaveToStream(stream);

                        _currentFile = path;
                    };
                   
                }
            });

            _form.AddComponate(new Button(new Rectangle(130, 640, 60, 30), _form, "Clear")
            {
                OnLeftClick = (object sender, Point pos)=>
                {
                    Controler.ShowMessageDialog("Are you sure", "you will lose any unsaved changes", new List<string>() { "Yes", "No" },
                        (object s) =>
                        {
                            if (s!= null)
                                if (s == "Yes")
                                {
                                    _effect = new ParticalEffect();
                                    _emitter = null;
                                    _currentFile = string.Empty;
                                    UpdateForm();

                                }
                        });
                }
            });

            _form.AddComponate(new Button(new Rectangle(10, 640, 60, 30), _form, "Save As")
            {
                OnLeftClick = (object sender, Point pos) =>
                {
                    _dialogSavingLoading.AllowedFileExtentions.Clear();
                    _dialogSavingLoading.AllowedFileExtentions.Add(".pe");
                    _dialogSavingLoading.ShowSaveDialog(true, _currentFile);


                    _dialogSavingLoading.OnFileSelected = (string path, FileDialogType type) =>
                    {
                        using (FileStream stream = File.Create(path))
                            _effect.SaveToStream(stream);

                        _currentFile = path;
                    };

                }
            });
            _form.AddComponate(new Button(new Rectangle(130, 680, 60, 30), _form, "Load")
            {
                OnLeftClick = (object sender, Point pos) =>
                {
                    _dialogSavingLoading.AllowedFileExtentions.Clear();
                    _dialogSavingLoading.AllowedFileExtentions.Add(".pe");
                    _dialogSavingLoading.ShowLoadDialog(true, _currentFile);
                    
                    _dialogSavingLoading.OnFileSelected = (string path, FileDialogType type) =>
                    {
                        using (FileStream stream = File.OpenRead(path))
                            _effect = ParticalEffect.LoadFromStream(stream, _device);

                        if (_effect.Emitters.Count > 0)
                            _emitter = _effect.Emitters[0];
                        UpdateForm();

                        _currentFile = path;
                    };
                }
            });
        }

        private void SetIndexToListBox(string listbox, int index)
        {
            ListBox lb_tmp = _form.FindComponateById(listbox) as ListBox;
            lb_tmp.Active = true;
            lb_tmp.SetSelectedItem(index);
        }

        private void SetValueToTextBox(string textbox, int value)
        {
            TextBox txt_tmp = _form.FindComponateById(textbox) as TextBox;
            txt_tmp.Active = true;
            txt_tmp.Text = value.ToString();
        }

        private void SetValueToTextBox(string textbox, Vector2 value)
        {
            TextBox txt_tmp = _form.FindComponateById(textbox) as TextBox;
            txt_tmp.Active = true;
            txt_tmp.Text =  $"{value.X}, {value.Y}";
        }

        private void SetValueToTextBox(string textbox, float value)
        {
            TextBox txt_tmp = _form.FindComponateById(textbox) as TextBox;
            txt_tmp.Active = true;
            txt_tmp.Text = value.ToString();
        }

        private int SetValueFromTextbox(TextBox tb, int min, int max)
        {
            if (tb.Text.Length <= 0)
                tb.Text = min.ToString();

            int tmp = 0;
            try
            {
                tmp = Convert.ToInt32(tb.Text);
                if (tmp < min)
                    tmp = min;
                if (tmp > max)
                    tmp = max;
            }
            catch
            { return 0; }
            tb.Text = tmp.ToString();
            return tmp;

            
        }

        private float SetValueFromTextbox(TextBox tb, float min, float max)
        {
            if (tb.Text.Length <= 0)
                tb.Text = min.ToString();

            float tmp = 0;
            try
            {
                tmp = (float)Convert.ToDouble(tb.Text);
                if (tmp < min)
                    tmp = min;
                if (tmp > max)
                    tmp = max;
            }
            catch
            { return 0; }
            tb.Text = tmp.ToString();
            return tmp;


        }

        private FloatRange SetFloatRangeFromTextbox(TextBox tb, float min, float max, FloatRange input, bool one)
        {
            if (tb.Text.Length <= 0)
                tb.Text = min.ToString();

            float tmp = (float)Convert.ToDouble(tb.Text);
            if (tmp < min)
                tmp = min;
            if (tmp > max)
                tmp = max;

            tb.Text = tmp.ToString();

            if (one)
                input.One = tmp;
            else
                input.Two = tmp;
            return input;


        }

        private Vector2 ConvertTextToVector2(string input)
        {
            if (input.Contains(","))
            {
                input = input.Replace(" ", "");
                float x = 0, y = 0;

                int index = input.IndexOf(",");
                try
                {
                    x = (float)Convert.ToDecimal(input.Substring(0, index));
                    y = (float)Convert.ToDecimal(input.Substring(index + 1));
                }
                catch
                {
                    x = 0;
                    y = 0;
                }

                return new Vector2(x, y);
            }

            return Vector2.Zero;
        }

        private Vector2Range SetVector2RangeFromTextbox(TextBox tb, float min, float max, Vector2Range input, bool one)
        {
            if (tb.Text.Length <= 0)
                tb.Text = min.ToString();

            Vector2 tmp = ConvertTextToVector2(tb.Text);
            if (tmp.X < min)
                tmp.X = min;
            if (tmp.X > max)
                tmp.X = max;

            if (tmp.Y < min)
                tmp.Y = min;
            if (tmp.Y > max)
                tmp.Y = max;

            SetValueToTextBox(tb.id, tmp);

            if (one)
            {
                input.One = tmp;

            }
            else
                input.Two = tmp;
            return input;


        }

        private Vector2 SetVector2FromTextbox(TextBox tb, float min, float max)
        {
            if (tb.Text.Length <= 0)
                tb.Text = min.ToString();

            Vector2 tmp = ConvertTextToVector2(tb.Text);
            if (tmp.X < min)
                tmp.X = min;
            if (tmp.X > max)
                tmp.X = max;

            if (tmp.Y < min)
                tmp.Y = min;
            if (tmp.Y > max)
                tmp.Y = max;

            //SetValueToTextBox(tb.id, tmp);

            
            return tmp;


        }


        private Texture2D _dot;

        private FileDialog _dialogFilePicker;
        private FileDialog _dialogFilePickerBackground;

        private FileDialog _dialogSavingLoading;

        private Texture2D LoadTextureForFile(string path)
        {
            Texture2D tmp = null;
            using (FileStream stream = File.OpenRead(path))
                tmp = Texture2D.FromStream(_device, stream);
            return tmp;
        }

        private void OnFileSelected(string path, FileDialogType type)
        {

            
            string r = Directory.GetCurrentDirectory();
            if (path.Contains(r))
            {
                path = path.Replace(r, ".");
            }
            else
            {
                Controler.ShowErrorDialog("Error", "Texture must be in a sub directory of the partical editor");

            }
            Texture2D tmp = LoadTextureForFile(path);
            _emitter.SetTexture(tmp, path);

            PictureBox pb = _form.FindComponateById("pb_texture") as PictureBox;
            pb.Img = tmp;

        }

        private GraphicsDevice _device;

        float tmp = 0;
        public void Intialize(Game g)
        {
            _haveIntialized = true;

            _device = g.GraphicsDevice;



            _dot = new Texture2D(g.GraphicsDevice, 1, 1);
            _dot.SetData<Color>(new Color[] { Color.White });

            _screenManiger = new Riddlersoft.Core.Screen.ScreenManiger(g);
            Controler.Intizalize(g.GraphicsDevice);
            Controler.LoadContent(g.Content);

            _gameScreen = new GameScreen(new Rectangle(0, 0, 1280, 720), null);
            _gameScreen.OnLeftMouseDown = (object sender, Point pos) =>
            {
                _effect.Trigger(pos, 1, tmp, .3f);
                tmp += .1f;
            };

            _gameScreen.OnRightClick = (object sender, Point pos) =>
            {
                _effect.Trigger(pos, 1);
            };

            _dialogFilePicker = new FileDialog(new Rectangle(200, 200, 500, 400), _gameScreen);
            _dialogFilePickerBackground = new FileDialog(new Rectangle(200, 200, 500, 400), _gameScreen);
            _dialogSavingLoading = new FileDialog(new Rectangle(200, 200, 500, 400), _gameScreen);

            _gameScreen.AddComponate(_dialogSavingLoading);

            _form = new Form(new Rectangle(0, 0, 200, 720), _gameScreen)
            {
                Title = "",
                HaveTitleBar = false,
                HaveCloseButton = false,
            };

            _frm_newModifyers = new Form(new Rectangle(300, 100, 360, 150), _gameScreen)
            {
                Title = "Add Moddifyers",
                Visible = false,
                Active = false,
                id = "frm_add_moddifyer",
            };

            _frm_EditModifyer = new Form(new Rectangle(0, 0, 280, 720), _gameScreen)
            {
                Title = "Edit Moddifyer",
                Visible = false,
                Active = false,
                id = "frm_edit_moddifyer",
            };



            _frm_newModifyers.AddComponate(new WinLable(new Vector2(10, 46), _frm_newModifyers, "Select Modifyer"));

            _frm_newModifyers.AddComponate(new Button(new Rectangle(150, 80, 60, 40), _frm_newModifyers, "Add")
            {
                CloseParent = true,
                OnLeftClick = (object sender, Point pos) =>
                {
                    ListBox lb = _frm_newModifyers.FindComponateById("lb_modifyer") as ListBox;
                    switch (lb.SelectedItem)
                    {
                        case 0:
                            _emitter.AddModifyer(new DampaningModifyer(.1f, _emitter)
                            {
                                DampenAngulorVelocity = true,
                                DampenVelocity = true,
                                DampaningAmount = .1f,
                                AngulorDampaningAmount = .1f,
                            });
                            break;
                        case 1:
                            /*_emitter.AddModifyer(new KillModifyer()
                            {
                                
                                LiveArea = new Rectangle(100, 100, 50, 50),
                                KillCheck = KillModifyer.KillDirectoin.All,
                            });*/
                            break;
                        case 2:
                            _emitter.AddModifyer(new LinearColourModifyer(_emitter)
                            {
                                InitialColor = Color.White,
                                EndColor = Color.Red,
                            });
                            break;
                        case 3:
                            _emitter.AddModifyer(new LinearGravityModifyer(_emitter)
                            {
                                Gravity = new Vector2(0, 1f),
                            });
                            break;
                        case 4:
                            _emitter.AddModifyer(new LinearFadeModifyer(_emitter)
                            {
                                EndFade = 0f,
                                InitialFade = 1f,
                            });
                            break;
                        case 5:
                            _emitter.AddModifyer(new LinearScaleModifyer(_emitter)
                            {
                                EndScale = 4,
                            });
                            break;
                        case 6:
                            _emitter.AddModifyer(new OsolatingModifyer()
                            {
                                Amount = 10,
                                Speed = 2,
                                YAxis = true,
                            });
                            break;
                        case 7:
                            _emitter.AddModifyer(new RotatingOsolatingModifyer()
                            {
                                Amount = 10,
                                Speed = 2,
                            });
                            break;
                        case 8:
                            _emitter.AddModifyer(new StateColourModifyer(_emitter)
                            {
                                EndColour = Color.Red,
                                InitialColor = Color.Green,
                                MiddleColour = Color.Blue,
                                MiddleColourPosition = .5f,
                            });
                            break;
                        case 9:
                            _emitter.AddModifyer(new StateFadeModifyer(_emitter)
                            {
                                InitialFade = 1,
                                MiddleStateFade = .8f,
                                EndFade = 0f,
                                MiddleLifeTime = .8f,
                            });
                            break;
                        default:
                            Modifyer m = new CustomModifyer();
                            m = m.Create(lb.CurrentItemName);
                            _emitter.AddModifyer(m);
                            break;
                    }
                    UpdateForm();
                }
            });

            _frm_newModifyers.AddComponate(new ListBox(new Rectangle(140, 40, 200, 30), _frm_newModifyers, new List<string>() { "Dampaning", "Kill (Not Implemented)", "LinearColour", "LinearGravity", "LinearFade", "LinearScale", "Osolation", "RotatingOsolating", "StateColour", "StateFade", "RotationVector" }, true)
            {
                id = "lb_modifyer"
            });


            Modifyer.OnSetUp = () =>
            {
                Modifyer.AddCustomModifyer(new RotationVectorModifyer());
            };
            ParticalEffect.IntializeCustomProperties();

            ListBox lbm = _frm_newModifyers.FindComponateById("lb_modifyer") as ListBox;

            foreach (KeyValuePair<string, CustomModifyer> key in Modifyer.CustomModfyers)
                lbm.Items.Add(new ListBox.ListBoxItem(key.Key, lbm.Font));



            UpdateForm();

            _gameScreen.AddComponate(_form);

            DropDownMenu ddm = new DropDownMenu(new Vector2(0, 0), new List<string>() { "File", "Clear", "Save", "Save As", "Load" }, _gameScreen)
            {
                OnLeftClick = (object sender, Point pos) =>
                {
                    Button b = sender as Button;

                    if (b.Text == "Save")
                    {
                        if (_currentFile != string.Empty)
                        {
                            using (FileStream stream = File.Create(_currentFile))
                                _effect.SaveToStream(stream);
                            return;
                        }
                        _dialogSavingLoading.AllowedFileExtentions.Clear();
                        _dialogSavingLoading.AllowedFileExtentions.Add(".pe");
                        _dialogSavingLoading.ShowSaveDialog(true, _currentFile);


                        _dialogSavingLoading.OnFileSelected = (string path, FileDialogType type) =>
                        {
                            using (FileStream stream = File.Create(path))
                                _effect.SaveToStream(stream);

                            _currentFile = path;
                        };
                    }

                    if (b.Text == "Clear")
                    {
                        Controler.ShowMessageDialog("Are you sure", "you will lose any unsaved changes", new List<string>() { "Yes", "No" },
                            (object s) =>
                            {
                                if (s != null)
                                    if (s == "Yes")
                                    {
                                        _effect = new ParticalEffect();
                                        _emitter = null;
                                        _currentFile = string.Empty;
                                        UpdateForm();
                                    }
                            });
                    }

                    if (b.Text == "Save As")
                    {

                        _dialogSavingLoading.AllowedFileExtentions.Clear();
                        _dialogSavingLoading.AllowedFileExtentions.Add(".pe");
                        _dialogSavingLoading.ShowSaveDialog(true, _currentFile);


                        _dialogSavingLoading.OnFileSelected = (string path, FileDialogType type) =>
                        {
                            using (FileStream stream = File.Create(path))
                                _effect.SaveToStream(stream);

                            _currentFile = path;
                        };

                    }

                    if (b.Text == "Load")
                    {


                        _dialogSavingLoading.AllowedFileExtentions.Clear();
                        _dialogSavingLoading.AllowedFileExtentions.Add(".pe");
                        _dialogSavingLoading.ShowLoadDialog(true, _currentFile);

                        _dialogSavingLoading.OnFileSelected = (string path, FileDialogType type) =>
                        {
                            using (FileStream stream = File.OpenRead(path))
                                _effect = ParticalEffect.LoadFromStream(stream, _device);

                            if (_effect.Emitters.Count > 0)
                                _emitter = _effect.Emitters[0];
                            UpdateForm();

                            _currentFile = path;
                        };
                    }

                },
            };

            DropDownMenu ddm2 = new DropDownMenu(new Vector2(ddm.Area.Width, 0), new List<String>() { "Background", "Clear", "Set" }, _gameScreen)
            {
                OnLeftClick = (object sender, Point pos) =>
                {
                    Button b = sender as Button;
                    if (b.Text == "Clear")
                        _background = null;

                    if (b.Text == "Set")
                    {
                        _dialogFilePickerBackground.OnFileSelected = (string path, FileDialogType type) =>
                        {
                            using (FileStream stream = File.OpenRead(path))
                                _background = Texture2D.FromStream(_device, stream);

                        };

                        _dialogFilePickerBackground.AllowedFileExtentions.Clear();
                        _dialogFilePickerBackground.AllowedFileExtentions.Add("*.png");
                        _dialogFilePickerBackground.ShowLoadDialog(true, string.Empty);
                    }
                },
            };


            DropDownMenu ddm3 = new DropDownMenu(new Vector2(ddm.Area.Width + ddm2.Area.Width, 0), new List<string>() { "BlendMode", "AlphaBlend", "Additive", "NonPremultiplied", "Opaque" }, _gameScreen)
            {
                OnLeftClick = (object sender, Point pos) =>
                {
                    Button b = sender as Button;
                    if (b.Text == "AlphaBlend")
                        _blendState = BlendState.AlphaBlend;

                    if (b.Text == "Additive")
                        _blendState = BlendState.Additive;

                    if (b.Text == "NonPremultiplied")
                        _blendState = BlendState.NonPremultiplied;

                    if (b.Text == "Opaque")
                        _blendState = BlendState.Opaque;
                }
            };


            _gameScreen.AddComponate(ddm);
            _gameScreen.AddComponate(ddm2);
            _gameScreen.AddComponate(ddm3);

            _gameScreen.AddComponate(_frm_newModifyers);

            _gameScreen.AddComponate(_frm_EditModifyer);


            _effect = new ParticalEffect();


            _frm_newEmmiter = new Form(new Rectangle(300, 100, 150, 180), _gameScreen)
            {
                Title = "Emmiter",
                Visible = false,
                Active = false,
                id = "frm_newemmiter"
            };
            _frm_newEmmiter.AddComponate(new TickBox(new Rectangle(80, 40, 10, 10), _frm_newEmmiter)
            {
                Text = "Circle",
                Ticked = true,
                TextOffset = new Vector2(0, 3),
                id = "t1",
                OnLeftClick = (object sender, Point pos) =>
                {
                    TickBox t = sender as TickBox;
                    t.Ticked = true;

                    TickBox ot = _frm_newEmmiter.FindComponateById("t2") as TickBox;
                    ot.Ticked = false;

                    ot = _frm_newEmmiter.FindComponateById("t3") as TickBox;
                    ot.Ticked = false;
                },
            });

            _frm_newEmmiter.AddComponate(new TickBox(new Rectangle(80, 70, 10, 10), _frm_newEmmiter)
            {
                Text = "Cone",
                Ticked = false,
                TextOffset = new Vector2(0, 3),
                id = "t2",
                OnLeftClick = (object sender, Point pos) =>
                {
                    TickBox t = sender as TickBox;
                    t.Ticked = true;

                    TickBox ot = _frm_newEmmiter.FindComponateById("t1") as TickBox;
                    ot.Ticked = false;

                    ot = _frm_newEmmiter.FindComponateById("t3") as TickBox;
                    ot.Ticked = false;
                },
            });


            _frm_newEmmiter.AddComponate(new TickBox(new Rectangle(80, 100, 10, 10), _frm_newEmmiter)
            {
                Text = "Rectangle",
                Ticked = false,
                TextOffset = new Vector2(0, 3),
                id = "t3",
                OnLeftClick = (object sender, Point pos) =>
                 {
                     TickBox t = sender as TickBox;
                     t.Ticked = true;

                     TickBox ot = _frm_newEmmiter.FindComponateById("t1") as TickBox;
                     ot.Ticked = false;

                     ot = _frm_newEmmiter.FindComponateById("t2") as TickBox;
                     ot.Ticked = false;
                 },
            });

            _frm_newEmmiter.AddComponate(new Button(new Rectangle(30, 130, 60, 30), _frm_newEmmiter, "Add")
            {
                CloseParent = true,
                OnLeftClick = (object sender, Point pos) =>
                {
                    TickBox circle = _frm_newEmmiter.FindComponateById("t1") as TickBox;
                    TickBox cone = _frm_newEmmiter.FindComponateById("t2") as TickBox;
                    Emitter em = null;
                    if (circle.Ticked)
                        em = new CircleEmitter(10)
                        {
                            Initial_Velocity = new Vector2Range(-new Vector2(1, 1), new Vector2(1, 1)),
                            ParticalSpeed = new FloatRange(1, 10),
                            Function = EmmiterFunction.Explosive,

                            ReleaseAmount = 5,
                            Initial_Colour = new ColourRange(Color.White, Color.Gray),
                            Initial_Rotaiton = 0f,
                            Initial_Scale = new FloatRange(1, 1),
                            Initial_LifeTime = 1f,
                            Initial_Angulor_Velocity = new FloatRange(0, 0),
                        };
                    else
                    if (cone.Ticked)
                        em = new ConeEmitter(10)
                        {
                            Initial_Velocity = new Vector2Range(-new Vector2(1, 1), new Vector2(1, 1)),
                            ParticalSpeed = new FloatRange(1, 10),
                            MaxRadious = MathHelper.PiOver2,
                            MinRadious = MathHelper.Pi,
                            Function = EmmiterFunction.Explosive,

                            ReleaseAmount = 5,
                            Initial_Colour = new ColourRange(Color.White, Color.Gray),
                            Initial_Rotaiton = 0f,
                            Initial_Scale = new FloatRange(1, 1),
                            Initial_LifeTime = 1f,
                            Initial_Angulor_Velocity = new FloatRange(0, 0),
                        };
                    else

                        em = new RectangleEmitter(new Rectangle(0, 0, 100, 100))
                        {
                            Initial_Velocity = new Vector2Range(-new Vector2(1, 1), new Vector2(1, 1)),

                            ReleaseAmount = 5,
                            Initial_Colour = new ColourRange(Color.White, Color.Gray),
                            Initial_Rotaiton = 0f,
                            Initial_Scale = new FloatRange(1, 1),
                            Initial_LifeTime = 1f,
                            Initial_Angulor_Velocity = new FloatRange(0, 0),
                        };

                    em.SetTexture(_dot, string.Empty);

                    if (!_effect.AddEmitter(em))
                    {
                        Controler.ShowErrorDialog("Error", "Max Emitters reached");
                        return;
                    }
                    _emitter = _effect.Emitters[_effect.Emitters.Count - 1];
                    UpdateForm();
                }
            });

            _gameScreen.AddComponate(_frm_newEmmiter);

            Controler.Add(_gameScreen);



        
        }


        public void SetFont(SpriteFont fontVSmall, SpriteFont fontSmall, SpriteFont fontNormal)
        {
            Fonts.VerySmall = fontVSmall;
            Fonts.Small = fontSmall;
            Fonts.Normal = fontNormal;
            Fonts.Large = fontNormal;
        }

        public void Update(float dt)
        {
            if (!_haveIntialized)
                throw new Exception("Please intialize first");

            Controler.Update(dt);

            _effect.Update(dt);
        }

        private BlendState _blendState = BlendState.AlphaBlend;

        public void Draw(SpriteBatch sb)
        {
            if (_background != null)
                sb.Draw(_background, new Vector2(0, 0), Color.White);

            sb.End();
            sb.Begin(SpriteSortMode.Texture, _blendState);
            _effect.Render(sb);
            sb.End();
            sb.Begin();

            Controler.Render(ref sb);

            Vector2 pos = new Vector2(1200, 620);
            if (_effect != null)
                foreach (Emitter e in _effect.Emitters)
                {
                    float per = e.Particals.Count / (float)e.MaxParticles;
                    if (per < .6f)
                        sb.DrawString(Fonts.Small, e.Particals.Count.ToString(), new Vector2(pos.X, pos.Y), Color.Green);
                    else
                        if (per < .9f)
                        sb.DrawString(Fonts.Small, e.Particals.Count.ToString(), new Vector2(pos.X, pos.Y), Color.White);
                    else
                        sb.DrawString(Fonts.Small, e.Particals.Count.ToString(), new Vector2(pos.X, pos.Y), Color.Red);
                    pos.Y += 20;
                }
        }
    }
}
