﻿
using System;



using System.IO;



using System.Runtime.InteropServices;



namespace BarMateTabletOrdering.Helpers
{
    public class RawPrinterHelper
    {



        // Structure and API declarions:



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]



        public class DOCINFOA
        {



            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;



            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;



            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;



        }



        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true,

               CharSet = CharSet.Ansi, ExactSpelling = true,

               CallingConvention = CallingConvention.StdCall)]



        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)]

                  string szPrinter, out IntPtr hPrinter, uint pd);



        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,

               ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]



        public static extern bool ClosePrinter(IntPtr hPrinter);



        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true,

               CharSet = CharSet.Ansi, ExactSpelling = true,

               CallingConvention = CallingConvention.StdCall)]



        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level,

              [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);



        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true,

               ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]



        public static extern bool EndDocPrinter(IntPtr hPrinter);



        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true,

               ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]



        public static extern bool StartPagePrinter(IntPtr hPrinter);



        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true,

               ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]



        public static extern bool EndPagePrinter(IntPtr hPrinter);



        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true,

               ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]



        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32

              dwCount, out Int32 dwWritten);



        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false,

               ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]



        public static extern Int32 GetLastError();



        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {



            Int32 dwError = 0, dwWritten = 0;



            IntPtr hPrinter = new IntPtr(0);



            DOCINFOA di = new DOCINFOA();



            bool bSuccess = false; // Assume failure unless you specifically succeed.



            di.pDocName = "My C#.NET RAW Document";



            di.pDataType = "RAW";



            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {



                if (StartDocPrinter(hPrinter, 1, di))
                {



                    if (StartPagePrinter(hPrinter))
                    {



                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);



                        EndPagePrinter(hPrinter);



                    }



                    EndDocPrinter(hPrinter);



                }



                ClosePrinter(hPrinter);



            }



            if (bSuccess == false)
            {



                dwError = GetLastError();



            }



            return bSuccess;



        }



        public static bool SendFileToPrinter(string szPrinterName, string

              szFileName)
        {



            FileStream fs = new FileStream(szFileName, FileMode.Open);



            BinaryReader br = new BinaryReader(fs);



            Byte[] bytes = new Byte[fs.Length];



            bool bSuccess = false;



            IntPtr pUnmanagedBytes = new IntPtr(0);



            int nLength;



            nLength = Convert.ToInt32(fs.Length);



            bytes = br.ReadBytes(nLength);



            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);



            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);



            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);



            Marshal.FreeCoTaskMem(pUnmanagedBytes);



            return bSuccess;



        }



        public static bool SendStringToPrinter(string szPrinterName, string

              szString)
        {



            IntPtr pBytes;



            Int32 dwCount;



            dwCount = szString.Length;



            pBytes = Marshal.StringToCoTaskMemAnsi(szString);



            SendBytesToPrinter(szPrinterName, pBytes, dwCount);



            Marshal.FreeCoTaskMem(pBytes);



            return true;



        }



        public static bool OpenCashDrawer1(string szPrinterName)
        {



            Int32 dwError = 0, dwWritten = 0;



            IntPtr hPrinter = new IntPtr(0);



            DOCINFOA di = new DOCINFOA();



            bool bSuccess = false;



            di.pDocName = "OpenDrawer";



            di.pDataType = "RAW";



            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {



                if (StartDocPrinter(hPrinter, 1, di))
                {



                    if (StartPagePrinter(hPrinter))
                    {



                        int nLength;



                        //byte[] DrawerOpen = new byte[] { 07 };
                        char V = 'V';
                        byte[] DrawerOpen = { 0x1B, 0x70, 0x0, 60, 120 };





                        nLength = DrawerOpen.Length;



                        IntPtr p = Marshal.AllocCoTaskMem(nLength);



                        Marshal.Copy(DrawerOpen, 0, p, nLength);



                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);



                        EndPagePrinter(hPrinter);



                        Marshal.FreeCoTaskMem(p);



                    }



                    EndDocPrinter(hPrinter);



                }



                ClosePrinter(hPrinter);



            }



            if (bSuccess == false)
            {



                dwError = GetLastError();



            }



            return bSuccess;



        }



        public static bool PrintMyOwn(string szPrinterName)
        {



            Int32 dwError = 0, dwWritten = 0;



            IntPtr hPrinter = new IntPtr(0);



            DOCINFOA di = new DOCINFOA();



            bool bSuccess = false;



            di.pDocName = "OpenDrawer";



            di.pDataType = "RAW";



            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {



                if (StartDocPrinter(hPrinter, 1, di))
                {



                    if (StartPagePrinter(hPrinter))
                    {



                        int nLength;



                        byte[] DrawerOpen = new byte[] { 07 };



                        nLength = DrawerOpen.Length;



                        IntPtr p = Marshal.AllocCoTaskMem(nLength);



                        Marshal.Copy(DrawerOpen, 0, p, nLength);



                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);



                        EndPagePrinter(hPrinter);



                        Marshal.FreeCoTaskMem(p);



                    }



                    EndDocPrinter(hPrinter);



                }



                ClosePrinter(hPrinter);



            }



            if (bSuccess == false)
            {



                dwError = GetLastError();



            }



            return bSuccess;

        }

        public static bool InitialisePrinter(string szPrinterName)
        {
            Int32 dwError = 0, dwWritten = 0;



            IntPtr hPrinter = new IntPtr(0);



            DOCINFOA di = new DOCINFOA();



            bool bSuccess = false;



            di.pDocName = "InitialisePrinter";



            di.pDataType = "RAW";



            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {



                if (StartDocPrinter(hPrinter, 1, di))
                {



                    if (StartPagePrinter(hPrinter))
                    {



                        int nLength;


                        char V = '@';
                        byte[] DrawerOpen = { 0x1d, Convert.ToByte(V) };



                        nLength = DrawerOpen.Length;



                        IntPtr p = Marshal.AllocCoTaskMem(nLength);



                        Marshal.Copy(DrawerOpen, 0, p, nLength);



                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);



                        EndPagePrinter(hPrinter);



                        Marshal.FreeCoTaskMem(p);



                    }



                    EndDocPrinter(hPrinter);



                }



                ClosePrinter(hPrinter);



            }



            if (bSuccess == false)
            {



                dwError = GetLastError();



            }



            return bSuccess;



        }

        public static bool DoAllNoCut(string szPrinterName)
        {
            Int32 dwError = 0, dwWritten = 0, dwWritten1 = 0, dwWritten2 = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            bool bSuccess1 = false;
            bool bSuccess2 = false;


            di.pDocName = "NoDrawer";
            di.pDataType = "RAW";
            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        int nLength;
                        int nLength1;
                        //int nLength2;


                        char V = '@';
                        char V1 = 'V';
                        byte[] DrawerOpen = { 0x1d, Convert.ToByte(V) };
                        byte[] DrawerOpen1 = { 0x1d, Convert.ToByte(V), 66, 0 };
                        //byte[] DrawerOpen2 = { 0x1B, 0x70, 0x0, 60, 120 };


                        nLength = DrawerOpen.Length;
                        nLength1 = DrawerOpen1.Length;
                        //nLength2 = DrawerOpen2.Length;


                        IntPtr p = Marshal.AllocCoTaskMem(nLength);
                        IntPtr p1 = Marshal.AllocCoTaskMem(nLength1);
                        //IntPtr p2 = Marshal.AllocCoTaskMem(nLength2);


                        Marshal.Copy(DrawerOpen, 0, p, nLength);
                        Marshal.Copy(DrawerOpen1, 0, p1, nLength1);
                        //Marshal.Copy(DrawerOpen2, 0, p2, nLength2);



                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);
                        bSuccess1 = WritePrinter(hPrinter, p1, DrawerOpen1.Length, out dwWritten1);
                        //bSuccess2 = WritePrinter(hPrinter, p2, DrawerOpen2.Length, out dwWritten2);



                        EndPagePrinter(hPrinter);
                        Marshal.FreeCoTaskMem(p);
                        Marshal.FreeCoTaskMem(p1);
                        //Marshal.FreeCoTaskMem(p2);


                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            if (bSuccess == false)
            {
                dwError = GetLastError();
            }
            return bSuccess;
        }



        public static bool DoAll(string szPrinterName)
        {
            Int32 dwError = 0, dwWritten = 0, dwWritten1 = 0, dwWritten2 = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            bool bSuccess1 = false;
            bool bSuccess2 = false;


            di.pDocName = "DoAll";
            di.pDataType = "RAW";
            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        int nLength;
                        int nLength1;
                        int nLength2;


                        char V = '@';
                        char V1 = 'V';
                        byte[] DrawerOpen = { 0x1d, Convert.ToByte(V) };
                        byte[] DrawerOpen1 = { 0x1d, Convert.ToByte(V), 66, 0 };
                        byte[] DrawerOpen2 = { 0x1B, 0x70, 0x0, 60, 120 };


                        nLength = DrawerOpen.Length;
                        nLength1 = DrawerOpen1.Length;
                        nLength2 = DrawerOpen2.Length;


                        IntPtr p = Marshal.AllocCoTaskMem(nLength);
                        IntPtr p1 = Marshal.AllocCoTaskMem(nLength1);
                        IntPtr p2 = Marshal.AllocCoTaskMem(nLength2);


                        Marshal.Copy(DrawerOpen, 0, p, nLength);
                        Marshal.Copy(DrawerOpen1, 0, p1, nLength1);
                        Marshal.Copy(DrawerOpen2, 0, p2, nLength2);



                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);
                        bSuccess1 = WritePrinter(hPrinter, p1, DrawerOpen1.Length, out dwWritten1);
                        bSuccess2 = WritePrinter(hPrinter, p2, DrawerOpen2.Length, out dwWritten2);



                        EndPagePrinter(hPrinter);
                        Marshal.FreeCoTaskMem(p);
                        Marshal.FreeCoTaskMem(p1);
                        Marshal.FreeCoTaskMem(p2);


                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            if (bSuccess == false)
            {
                dwError = GetLastError();
            }
            return bSuccess;
        }

        public static bool DoAllNoInit(string szPrinterName)
        {
            Int32 dwError = 0, dwWritten = 0, dwWritten1 = 0, dwWritten2 = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            bool bSuccess1 = false;
            bool bSuccess2 = false;


            di.pDocName = "DoAllNoInit";
            di.pDataType = "RAW";
            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        //int nLength;
                        int nLength1;
                        int nLength2;


                        char V = '@';
                        char V1 = 'V';
                        byte[] DrawerOpen = { 0x1d, Convert.ToByte(V) };
                        byte[] DrawerOpen1 = { 0x1d, Convert.ToByte(V), 66, 0 };
                        byte[] DrawerOpen2 = { 0x1B, 0x70, 0x0, 60, 120 };


                        //nLength = DrawerOpen.Length;
                        nLength1 = DrawerOpen1.Length;
                        nLength2 = DrawerOpen2.Length;


                        //IntPtr p = Marshal.AllocCoTaskMem(nLength);
                        IntPtr p1 = Marshal.AllocCoTaskMem(nLength1);
                        IntPtr p2 = Marshal.AllocCoTaskMem(nLength2);


                        //Marshal.Copy(DrawerOpen, 0, p, nLength);
                        Marshal.Copy(DrawerOpen1, 0, p1, nLength1);
                        Marshal.Copy(DrawerOpen2, 0, p2, nLength2);



                        //bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);
                        bSuccess1 = WritePrinter(hPrinter, p1, DrawerOpen1.Length, out dwWritten1);
                        bSuccess2 = WritePrinter(hPrinter, p2, DrawerOpen2.Length, out dwWritten2);



                        EndPagePrinter(hPrinter);
                        //Marshal.FreeCoTaskMem(p);
                        Marshal.FreeCoTaskMem(p1);
                        Marshal.FreeCoTaskMem(p2);


                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            if (bSuccess == false)
            {
                dwError = GetLastError();
            }
            return bSuccess;
        }


        public static bool DoSomeThing(string szPrinterName, byte[] DrawerOpen)
        {

            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            di.pDocName = "FullCut";
            di.pDataType = "RAW";
            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        int nLength;
                        //byte[] DrawerOpen = new byte[] { 27, 100, 51 };
                        //char V = 'V';
                        //byte[] DrawerOpen = { 0x1d, Convert.ToByte(V), 66, 0 };
                        nLength = DrawerOpen.Length;
                        IntPtr p = Marshal.AllocCoTaskMem(nLength);
                        Marshal.Copy(DrawerOpen, 0, p, nLength);
                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);
                        EndPagePrinter(hPrinter);
                        Marshal.FreeCoTaskMem(p);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            if (bSuccess == false)
            {
                dwError = GetLastError();
            }
            return bSuccess;
        }


        public static bool FullCut(string szPrinterName)
        {

            Int32 dwError = 0, dwWritten = 0;



            IntPtr hPrinter = new IntPtr(0);



            DOCINFOA di = new DOCINFOA();



            bool bSuccess = false;



            di.pDocName = "FullCut";



            di.pDataType = "RAW";



            if (OpenPrinter(szPrinterName, out hPrinter, 0))
            {



                if (StartDocPrinter(hPrinter, 1, di))
                {



                    if (StartPagePrinter(hPrinter))
                    {



                        int nLength;



                        //byte[] DrawerOpen = new byte[] { 27, 100, 51 };

                        char V = 'V';
                        byte[] DrawerOpen = { 0x1d, Convert.ToByte(V), 66, 0 };



                        nLength = DrawerOpen.Length;



                        IntPtr p = Marshal.AllocCoTaskMem(nLength);



                        Marshal.Copy(DrawerOpen, 0, p, nLength);



                        bSuccess = WritePrinter(hPrinter, p, DrawerOpen.Length, out dwWritten);



                        EndPagePrinter(hPrinter);



                        Marshal.FreeCoTaskMem(p);



                    }



                    EndDocPrinter(hPrinter);



                }



                ClosePrinter(hPrinter);



            }



            if (bSuccess == false)
            {



                dwError = GetLastError();



            }



            return bSuccess;



        }



    }
}