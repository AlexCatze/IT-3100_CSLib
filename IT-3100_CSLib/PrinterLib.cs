using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IT_3100_CsLib
{
    public class PrinterLib
    {
        const string PRINTER_LIB = "prnlib.dll";
        /*
         1    0 00001A89 PRNBMPOut//
         2    1 000019DD PRNBarcodeOut//
         3    2 00001B05 PRNCheckMarker//
         4    3 00001689 PRNClose//
         5    4 000020BD PRNGetDrverVersion
         6    5 00001BED PRNGetLastError//
         7    6 00002049 PRNGetLogData
         8    7 00001CD9 PRNGetPaperWidth//
         9    8 00001FB9 PRNGetPrintData
        10    9 00001F41 PRNGetPrintDataHeigth
        11    A 00001DF9 PRNGetPrinterProperty//
        12    B 00002155 PRNGetPrinterStatus
        13    C 00001B79 PRNGetStatus//
        14    D 00001941 PRNImageOut//
        15    E 00001751 PRNInitializePrinter//
        16    F 000014D1 PRNOpen//
        17   10 000017C5 PRNPrintScreen//
        18   11 00001839 PRNPrintWindow//
        19   12 00001EB1 PRNRegisterParameterTabel
        20   13 00001C61 PRNSetPaperWidth//
        21   14 00001D51 PRNSetPrinterProperty//
        22   15 000018B1 PRNTextOut//
       */

        public enum PRNStatus : uint {
            PRN_NORMAL,
        	PRN_NOTOPEN,
            PRN_DRIVER_NOTEXIST,
        	PRN_ALREADY_OPEN,
        	PRN_NOTFOUND,
        	PRN_NOTCHANGE,
        	PRN_FILE_NOTEXIST,
        	PRN_FILEOPEN_ERROR,
        	PRN_FILEFORMAT_ERROR,
        	PRN_PARAMETER_ERROR,
        	PRN_HARDWARE_ERROR,
        	PRN_PLATEN_OPEN,
        	PRN_PAPER_END,
        	PRN_VDETP_OCCURRED,
        	PRN_SUSPEND_OCCURRED,
        	PRN_HEADTEMP_ERROR,
	        PRN_AUTOLOADING,
            PRN_FEEDKEY_ERROR
        }

        /*
        0	PRN_NORMAL : Normal end.
        1	PRN_NOTOPEN : Printer has not been set operable.
        //  PRN_DRIVER_NOTEXIST : Printer driver does not reside causing the printer not to be operable.
        3	PRN_ALREADY_OPEN : Already opened or occupied by other application software.
        	PRN_NOTFOUND : Marker cannot be found.
        	PRN_NOTCHANGE : Paper width cannot be changed.
        6	PRN_FILE_NOTEXIST : Specified file cannot be found.
        7	PRN_FILEOPEN_ERROR : Specified file cannot be opened.
        	PRN_FILEFORMAT_ERROR : Specified file is with illegal format.
        9	PRN_PARAMETER_ERROR : Parameter error.
        	PRN_HARDWARE_ERROR : Hardware of the printer is malfunction.
        11	PRN_PLATEN_OPEN : Platen of the printer is kept open.
        12	PRN_PAPER_END : Paper is not loaded.
        	PRN_VDETP_OCCURRED : VDETP has occurred.
        	PRN_SUSPEND_OCCURRED : Suspend has occurred.
        	PRN_HEADTEMP_ERROR : Error has occurred due to abonormal temperature in the printer head.
	        PRN_AUTOLOADING : Paper is being loaded automatically.
        17  PRN_FEEDKEY_ERROR : Paper feeding with the feed key is in operation causing the printer not to be operable.
         */

        public enum paperWidth : uint
        {
            WIDE80mm,
            STRAIT58mm
        }

        public enum codeType : uint
        {
            JAN,
            NW7,
            Code39,
            ITF,
            UPCE,
            Code128
        }

        public enum fontType : uint
        {
            NONE,
            ANK8x16,
            ANK6x7,
            OCRBI
        }

        public enum direction : uint
        {
            VERTICAL,
            HORIZONTAL
        }

        public enum paperType : uint
        {
            F200U9W5,
            HS360,
            AFP235,
            HG56S,
            TLC00,
            CUSTOM
        }

        public enum printMode : uint
        {
            SPEED,
            QUALITY,
            GRAPHIC
        }

        public struct printerSettings
        {
            public paperType dwPaperType;
            public uint dwDepth;
            public printMode dwSpeed;
            public bool dwAutoloading;
            public uint dwAutolodingLength;
            public bool dwPreheat;
            public bool dwPrintContinuation;
        }

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNOpen();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNClose();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNPrintScreen();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNInitializePrinter();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNPrintWindow(IntPtr hWindow);

        [DllImport(PRINTER_LIB, CharSet = CharSet.Auto)]
        private static extern PRNStatus PRNTextOut(uint dwLength, string szTextData);

        public static PRNStatus PRNTextOut(string szTextData)
        {
            string text = szTextData + '\n';
            return PRNTextOut((uint)text.Length, text);
        }

        public static PRNStatus SkipLines(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                PRNStatus res = PRNTextOut("\n");
                if (res != PRNStatus.PRN_NORMAL) return res;
            }
            return PRNStatus.PRN_NORMAL;
        }

        /*
        public static PRNStatus PRNTextOut(string szTextData)
        {
            unsafe
            {
                fixed (char* charr = (szTextData + '\0').ToCharArray())
                    return PRNTextOut(0, charr);
            }
        }*/

        //public static PRNStatus PRNTextOut(string szTextData)
        //{ return PRNTextOut(0, szTextData); }

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNImageOut(uint dwWidth, uint dwHeight, uint dwFeedLength, byte[] pbyImageData);

        [DllImport(PRINTER_LIB,CharSet=CharSet.Unicode)]
        private static extern PRNStatus PRNBarcodeOut(uint dwCode, uint dwHeight, bool dwCheckDigit, uint dwFont, uint dwLeftMargin, uint dwDirection, uint dwLength, string szBarcodeData);

        public static PRNStatus PRNBarcodeOut(codeType dwCode, uint dwHeight, bool dwCheckDigit, fontType dwFont, uint dwLeftMargin, direction dwDirection, string szBarcodeData)
        {
            return PRNBarcodeOut((uint)dwCode, dwHeight, dwCheckDigit, (uint)dwFont, dwLeftMargin, (uint)dwDirection, (uint)szBarcodeData.Length, szBarcodeData);
        }

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNBMPOut(string szFilename);

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNCheckMarker();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNGetStatus();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNGetLastError();

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNSetPaperWidth(paperWidth dwWidth);

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNGetPaperWidth(ref paperWidth dwWidth);

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNSetPrinterProperty(paperType dwPaperType, uint dwDepth, printMode dwSpeed, bool dwAutoloading, uint dwAutolodingLength, bool dwPreheat, bool dwPrintContinuation);

        public static PRNStatus PRNSetPrinterProperty(printerSettings prnSet)
        {
            return PRNSetPrinterProperty(prnSet.dwPaperType, prnSet.dwDepth, prnSet.dwSpeed, prnSet.dwAutoloading, prnSet.dwAutolodingLength, prnSet.dwPreheat, prnSet.dwPrintContinuation);
        }

        [DllImport(PRINTER_LIB)]
        public static extern PRNStatus PRNGetPrinterProperty(ref paperType dwPaperType, ref uint dwDepth, ref printMode dwSpeed, ref bool dwAutoloading, ref uint dwAutolodingLength, ref bool dwPreheat, ref bool dwPrintContinuation);

        public static PRNStatus PRNGetPrinterProperty(ref printerSettings prnSet)
        {
            return PRNGetPrinterProperty(ref prnSet.dwPaperType, ref prnSet.dwDepth, ref prnSet.dwSpeed, ref prnSet.dwAutoloading, ref prnSet.dwAutolodingLength, ref prnSet.dwPreheat, ref prnSet.dwPrintContinuation);
        }
    }
}
