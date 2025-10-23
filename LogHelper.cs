using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinLogHelper
{
    using System.Text;

    public static class LogHelper
    {
        public static StringBuilder LogBuilder { get; private set; } = new StringBuilder();

        public static void Ekle(string mesaj)
        {
            LogBuilder.AppendLine(mesaj);
        }

        public static void Temizle()
        {
            LogBuilder.Clear();
        }
    }
}
