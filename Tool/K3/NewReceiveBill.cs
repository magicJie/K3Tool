using System;

namespace Tool.K3
{
    /// <summary>
    /// 收款单
    /// </summary>
    public class NewReceiveBill
    {
        /// <summary>
        /// 表头
        /// </summary>
        public class Head
        {
            public static string TableName = "t_RP_NewReceiveBill";

            #region 字段
            /// <summary>
            /// 单据号
            /// </summary>
            protected string _FNumber;
            /// <summary>
            /// 单据日期
            /// </summary>
            protected DateTime _FDate;
            /// <summary>
            /// 财务日期
            /// </summary>
            protected DateTime _FFincDate;
            /// <summary>
            /// 汇率类型
            /// </summary>
            protected int _FExchangeRateType;
            /// <summary>
            /// 币别
            /// </summary>
            protected int _FCurrencyID;
            /// <summary>
            /// 汇率
            /// </summary>
            protected float _FExchangeRate;
            /// <summary>
            /// 核算项目
            /// </summary>
            protected int _FCustomer;
            /// <summary>
            /// 系统类型
            /// </summary>
            protected int _FRP;
            /// <summary>
            /// 内码
            /// </summary>
            protected int _FBillID;

            protected string _FReceiveAmount;
            protected string _FReceiveAmountFor;
            protected string _FSettleAmount;
            protected string _FSettleAmountFor;
            protected int _FBillType;
            protected string _FClassTypeID;
            protected float _FAdjustExchangeRate;
            #endregion
            #region 属性
            /// <summary>
            /// 打印次数
            /// </summary>
            public int FPrintCount
            {
                get { return 0; }
            }
            /// <summary>
            /// 单据号
            /// </summary>
            public string FNumber
            {
                get { return GetFNumber(); }
                set { _FNumber = value; }
            }
            /// <summary>
            /// 单据日期
            /// </summary>
            public DateTime FDate
            {
                get { return GetFDate(); }
                set { _FDate = value; }
            }
            /// <summary>
            /// 财务日期
            /// </summary>
            public DateTime FFincDate
            {
                get { return GetFFincDate(); }
                set { _FFincDate = value; }
            }
            /// <summary>
            /// 汇率类型
            /// </summary>
            public int FExchangeRateType
            {
                get { return GetFExchangeRateType(); }
                set { _FExchangeRateType = value; }
            }
            /// <summary>
            /// 币别
            /// </summary>
            public int FCurrencyID
            {
                get { return GetFCurrencyID(); }
                set { _FCurrencyID = value; }
            }
            /// <summary>
            /// 汇率
            /// </summary>
            public float FExchangeRate
            {
                get { return GetFExchangeRate(); }
                set { _FExchangeRate = value; }
            }
            /// <summary>
            /// 核算项目
            /// </summary>
            public int FCustomer
            {
                get { return GetFCustomer(); }
                set { _FCustomer = value; }
            }
            /// <summary>
            /// 系统类型
            /// </summary>
            public int FRP
            {
                get { return GetFRP(); }
                set { _FRP = value; }
            }

            //下面属性非金蝶表结构要求必填但是业务上要求必填
            /// <summary>
            /// 内码
            /// </summary>
            public int FBillID
            {
                get { return GetFBillID(); }
                set { _FBillID = value; }
            }

            public string FReceiveAmount
            {
                get { return GetFReceiveAmount(); }
                set { _FReceiveAmount = value; }
            }
            public string FReceiveAmountFor
            {
                get { return GetFReceiveAmountFor(); }
                set { _FReceiveAmountFor = value; }
            }
            public string FSettleAmount
            {
                get { return GetFSettleAmount(); }
                set { _FSettleAmount = value; }
            }
            public string FSettleAmountFor
            {
                get { return GetFSettleAmountFor(); }
                set { _FSettleAmountFor = value; }
            }
            public int FBillType
            {
                get { return GetFBillType(); }
                set { _FBillType = value; }
            }
            public string FClassTypeID
            {
                get { return GetFClassTypeID(); }
                set { _FClassTypeID = value; }
            }
            public float FAdjustExchangeRate
            {
                get { return GetFAdjustExchangeRate(); }
                set { _FAdjustExchangeRate = value; }
            }
            #endregion
            #region 虚方法
            /// <summary>
            /// 获取单据号
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFNumber()
            {
                return _FNumber;
            }
            /// <summary>
            /// 获取制单日期
            /// </summary>
            /// <returns></returns>
            protected virtual DateTime GetFDate()
            {
                return _FDate;
            }
            /// <summary>
            /// 获取财务日期
            /// </summary>
            /// <returns></returns>
            protected virtual DateTime GetFFincDate()
            {
                return _FFincDate;
            }
            /// <summary>
            /// 获取汇率类型
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFExchangeRateType()
            {
                return _FExchangeRateType;
            }
            /// <summary>
            /// 获取币别
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFCurrencyID()
            {
                return _FCurrencyID;
            }
            /// <summary>
            /// 获取汇率
            /// </summary>
            /// <returns></returns>
            protected virtual float GetFExchangeRate()
            {
                return _FExchangeRate;
            }
            /// <summary>
            /// 获取核算项目
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFCustomer()
            {
                return _FCustomer;
            }
            /// <summary>
            /// 获取系统类型
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFRP()
            {
                return _FRP;
            }
            /// <summary>
            /// 获取内码
            /// </summary>
            /// <returns></returns>
            protected int GetFBillID()
            {
                return _FBillID;
            }

            protected string GetFReceiveAmount()
            {
                return _FReceiveAmount;
            }
            protected string GetFReceiveAmountFor()
            {
                return _FReceiveAmountFor;
            }
            protected string GetFSettleAmount()
            {
                return _FSettleAmount;
            }
            protected string GetFSettleAmountFor()
            {
                return _FSettleAmountFor;
            }
            protected int GetFBillType()
            {
                return _FBillType;
            }
            protected string GetFClassTypeID()
            {
                return _FClassTypeID;
            }
            protected float GetFAdjustExchangeRate()
            {
                return _FAdjustExchangeRate;
            }
            #endregion
        }
        /// <summary>
        /// 表体
        /// </summary>
        public class Body
        {
            public static string TableName = "t_RP_ARBillOfSH";

            #region 字段
            /// <summary>
            /// 
            /// </summary>
            protected int _FBillID;
            ///// <summary>
            ///// 
            ///// </summary>
            //protected int _FEntryID;
            /// <summary>
            /// 
            /// </summary>
            protected int _FReceiveCyID;
            /// <summary>
            /// 
            /// </summary>
            protected decimal _FReceiveAmount;
            /// <summary>
            /// 
            /// </summary>
            protected decimal _FReceiveAmountFor;
            /// <summary>
            /// 
            /// </summary>
            protected float _FReceiveExchangeRate;
            /// <summary>
            /// 
            /// </summary>
            protected int _FSettleCyID;
            /// <summary>
            /// 
            /// </summary>
            protected decimal _FSettleAmountFor;
            /// <summary>
            /// 
            /// </summary>
            protected decimal _FSettleAmount;
            /// <summary>
            /// 
            /// </summary>
            protected float _FSettleExchangeRate;
            #endregion

            #region 属性
            /// <summary>
            /// 
            /// </summary>
            public int FBillID { get { return GetFBillID(); } set { _FBillID = value; } }
            ///// <summary>
            ///// 
            ///// </summary>
            //public int FEntryID { get { return GetFEntryID(); } set { _FEntryID = value; } }
            /// <summary>
            /// 
            /// </summary>
            public int FReceiveCyID { get { return GetFReceiveCyID(); } set { _FReceiveCyID = value; } }
            /// <summary>
            /// 
            /// </summary>
            public decimal FReceiveAmount { get { return GetFReceiveAmount(); } set { _FReceiveAmount = value; } }
            /// <summary>
            /// 
            /// </summary>
            public decimal FReceiveAmountFor { get { return GetFReceiveAmountFor(); } set { _FReceiveAmountFor = value; } }
            /// <summary>
            /// 
            /// </summary>
            public float FReceiveExchangeRate { get { return GetFReceiveExchangeRate(); } set { _FReceiveExchangeRate = value; } }
            /// <summary>
            /// 
            /// </summary>
            public int FSettleCyID { get { return GetFSettleCyID(); } set { _FSettleCyID = value; } }
            /// <summary>
            /// 
            /// </summary>
            public decimal FSettleAmountFor { get { return GetFSettleAmountFor(); } set { _FSettleAmountFor = value; } }
            /// <summary>
            /// 
            /// </summary>
            public decimal FSettleAmount { get { return GetFSettleAmount(); } set { _FSettleAmount = value; } }
            /// <summary>
            /// 
            /// </summary>
            public float FSettleExchangeRate { get { return GetFSettleExchangeRate(); } set { _FSettleExchangeRate = value; } }

            #endregion

            #region 虚方法
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFBillID()
            {
                return _FBillID;
            }
            ///// <summary>
            ///// 
            ///// </summary>
            ///// <returns></returns>
            //protected virtual int GetFEntryID()
            //{
            //    return _FEntryID;
            //}
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFReceiveCyID()
            {
                return _FReceiveCyID;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual decimal GetFReceiveAmount()
            {
                return _FReceiveAmount;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual decimal GetFReceiveAmountFor()
            {
                return _FReceiveAmountFor;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual float GetFReceiveExchangeRate()
            {
                return _FReceiveExchangeRate;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFSettleCyID()
            {
                return _FSettleCyID;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual decimal GetFSettleAmountFor()
            {
                return _FSettleAmountFor;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual decimal GetFSettleAmount()
            {
                return _FSettleAmount;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected virtual float GetFSettleExchangeRate()
            {
                return _FSettleExchangeRate;
            }
            #endregion
        }
    }
}