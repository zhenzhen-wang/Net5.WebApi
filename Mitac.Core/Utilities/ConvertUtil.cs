using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Mitac.Core.Attributes;
using System;

namespace Mitac.Core.Utilities
{
    public class ConvertUtil
    {
        /// <summary>
        /// 简繁体转换
        /// </summary>
        /// <param name="x"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ChineseConvert(string text, string type)
        {
            String value = String.Empty;
            switch (type)
            {
                case "1": //转简体
                    value = ChineseConverter.Convert(text, ChineseConversionDirection.TraditionalToSimplified);
                    break;
                case "2": //转繁体
                    value = ChineseConverter.Convert(text, ChineseConversionDirection.SimplifiedToTraditional);
                    break;
                default:
                    break;
            }
            return value;
        }

        //反射出实体类的属性名
        public string getAttribute<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }

            //Type tt = typeof(T);
            object[] attrs = t.GetType().GetCustomAttributes(false);
            foreach (var item in attrs)
            {
                if (item is ParamAttribute)
                {
                    tStr = (item as ParamAttribute).Remark;
                }

            }

            return tStr;
        }
    }
}
