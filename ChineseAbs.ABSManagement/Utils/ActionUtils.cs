using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagement.Utils
{
    /// <summary>
    /// Action in MVC
    /// </summary>
    public static class ActionUtils
    {
        #region Action Try Catch
        private static KeyValuePair<int, object> TryCatch(Func<KeyValuePair<int, object>> func)
        {
            try { return func(); }
            catch (ApplicationException ae)
            {
                var msgDetail = new MessageDetail(ae.Message, ae.StackTrace);
                return ActionUtils.Failure(msgDetail);
            }
            catch (Exception e)
            {
                var msgDetail = new MessageDetail(e.Message, e.StackTrace);
                return ActionUtils.Failure(msgDetail);
            }
        }

        private static KeyValuePair<int, object> TryCatch<T1>(Func<T1, KeyValuePair<int, object>> func, T1 param1)
        {
            try { return func(param1); }
            catch (ApplicationException ae) { return ActionUtils.Failure(new MessageDetail(ae.Message, ae.StackTrace)); }
            catch (Exception e) { return ActionUtils.Failure(new MessageDetail(e.Message, e.StackTrace)); }
        }

        private static KeyValuePair<int, object> TryCatch<T1, T2>(Func<T1, T2, KeyValuePair<int, object>> func, T1 param1, T2 param2)
        {
            try { return func(param1, param2); }
            catch (ApplicationException ae) { return ActionUtils.Failure(new MessageDetail(ae.Message, ae.StackTrace)); }
            catch (Exception e) { return ActionUtils.Failure(new MessageDetail(e.Message, e.StackTrace)); }
        }

        private static KeyValuePair<int, object> TryCatch<T1, T2, T3>(Func<T1, T2, T3, KeyValuePair<int, object>> func, T1 param1, T2 param2, T3 param3)
        {
            try { return func(param1, param2, param3); }
            catch (ApplicationException ae) { return ActionUtils.Failure(new MessageDetail(ae.Message, ae.StackTrace)); }
            catch (Exception e) { return ActionUtils.Failure(new MessageDetail(e.Message, e.StackTrace)); }
        }

        private static KeyValuePair<int, object> TryCatch<T1, T2, T3, T4>(Func<T1, T2, T3, T4, KeyValuePair<int, object>> func, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            try { return func(param1, param2, param3, param4); }
            catch (ApplicationException ae) { return ActionUtils.Failure(new MessageDetail(ae.Message, ae.StackTrace)); }
            catch (Exception e) { return ActionUtils.Failure(new MessageDetail(e.Message, e.StackTrace)); }
        }

        private static KeyValuePair<int, object> TryCatch<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, KeyValuePair<int, object>> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            try { return func(param1, param2, param3, param4, param5); }
            catch (ApplicationException ae) { return ActionUtils.Failure(new MessageDetail(ae.Message, ae.StackTrace)); }
            catch (Exception e) { return ActionUtils.Failure(new MessageDetail(e.Message, e.StackTrace)); }
        }
        #endregion

        #region Json Result
        public static ActionResult Json(Func<KeyValuePair<int, object>> func)
        {
            var result = TryCatch(func);
            var jsonResult = new JsonResult();
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.Data = result;
            return jsonResult;
        }
        #endregion

        #region Action Try Catch
        public static KeyValuePair<int, object> Success(object obj)
        {
            return new KeyValuePair<int, object>(0, obj);
        }

        public static KeyValuePair<int, object> Failure(object obj, int errorCode = 1)
        {
            return new KeyValuePair<int, object>(errorCode, obj);
        }
        #endregion

        class MessageDetail
        {
            public string Message { get; set; }
            public string StackTrace { get; set; }

            public MessageDetail(string msg, string stack)
            {
                Message = "[服务器错误：" + msg + "]";
                StackTrace = stack;
            }
        }
    }
}
