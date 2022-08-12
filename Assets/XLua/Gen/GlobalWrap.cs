#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class GlobalWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Global);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 10, 1, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "FindChild", _m_FindChild_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetMinuteTime", _m_GetMinuteTime_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "NumToChinese", _m_NumToChinese_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetRandomSequence", _m_GetRandomSequence_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Timing", _m_Timing_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Timing1", _m_Timing1_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Delay", _m_Delay_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnityWebRequestGetData", _m_UnityWebRequestGetData_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnityWebRequestGetSprite", _m_UnityWebRequestGetSprite_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "sprite1", _g_get_sprite1);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "sprite1", _s_set_sprite1);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Global does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindChild_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Transform _trans = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    string _childName = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = Global.FindChild( _trans, _childName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetMinuteTime_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _time = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        var gen_ret = Global.GetMinuteTime( _time );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_NumToChinese_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _x = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = Global.NumToChinese( _x );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetRandomSequence_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _total = LuaAPI.xlua_tointeger(L, 1);
                    int _count = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = Global.GetRandomSequence( _total, _count );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Timing_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.UI.Text>(L, 1)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.UI.Text _text = (UnityEngine.UI.Text)translator.GetObject(L, 1, typeof(UnityEngine.UI.Text));
                    UnityEngine.Events.UnityAction _action = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    int _time = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = Global.Timing( _text, _action, _time );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.UI.Text>(L, 1)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)) 
                {
                    UnityEngine.UI.Text _text = (UnityEngine.UI.Text)translator.GetObject(L, 1, typeof(UnityEngine.UI.Text));
                    UnityEngine.Events.UnityAction _action = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    
                        var gen_ret = Global.Timing( _text, _action );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Global.Timing!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Timing1_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.UI.Text>(L, 1)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.UI.Text _text = (UnityEngine.UI.Text)translator.GetObject(L, 1, typeof(UnityEngine.UI.Text));
                    UnityEngine.Events.UnityAction _action = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    int _time = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = Global.Timing1( _text, _action, _time );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.UI.Text>(L, 1)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)) 
                {
                    UnityEngine.UI.Text _text = (UnityEngine.UI.Text)translator.GetObject(L, 1, typeof(UnityEngine.UI.Text));
                    UnityEngine.Events.UnityAction _action = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    
                        var gen_ret = Global.Timing1( _text, _action );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Global.Timing1!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Delay_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)) 
                {
                    float _time = (float)LuaAPI.lua_tonumber(L, 1);
                    UnityEngine.Events.UnityAction _unityAction = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    
                        var gen_ret = Global.Delay( _time, _unityAction );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float _time = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        var gen_ret = Global.Delay( _time );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Global.Delay!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnityWebRequestGetData_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.UI.Image __imageComp = (UnityEngine.UI.Image)translator.GetObject(L, 1, typeof(UnityEngine.UI.Image));
                    string __url = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = Global.UnityWebRequestGetData( __imageComp, __url );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnityWebRequestGetSprite_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Sprite _sprite = (UnityEngine.Sprite)translator.GetObject(L, 1, typeof(UnityEngine.Sprite));
                    string __url = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = Global.UnityWebRequestGetSprite( _sprite, __url );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_sprite1(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Global.sprite1);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_sprite1(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    Global.sprite1 = (UnityEngine.Sprite)translator.GetObject(L, 1, typeof(UnityEngine.Sprite));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
