using System;
using System.Runtime.Serialization;

namespace Pokemon
{
    class InvalidStatusException : Exception
    {
        public InvalidStatusException() 
            : base()
        {
        }

        public InvalidStatusException(string message)
            : base(message)
        {
        }

        public InvalidStatusException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        //逆シリアル化コンストラクタ。このクラスの逆シリアル化のために必須。
        //アクセス修飾子をpublicにしないこと！（詳細は後述）
        protected InvalidStatusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
