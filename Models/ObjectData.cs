using System;
using System.Linq;
using System.Text;

namespace FinderOfStandarts.Models
{
    class ObjectData
    {
        public int Index { get; set; }
        public decimal[] Data { get; set; }

        public decimal this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public override string ToString()
        {
            return $"Object {Index} : [{string.Join(", ", Data)}]";
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var o = obj as ObjectData;
            if (o.Data.Length != Data.Length)
                return false;

            if (Index == o.Index)
                return true;

            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] != o.Data[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}