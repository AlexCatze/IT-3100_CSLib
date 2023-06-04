using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IT_3100_CsLib
{
    public class CMOSLib
    {
        const string CMOS_LIB = "IMGdecodece.dll";

        static private int stream_x = 0, stream_y = 0;

        public enum ImageFormat
        {
            IMAGE_2MONO,
            UNDEFINED,
            IMAGE_256MONO
        }

        public enum CMOSStatus : int
        {///NOT CONFIRMED
            IMG_SUCCESS
        }
        /*
            IMG_SUCCESS : Normal termination
            IMG_ERR_MEMORY : Memory error has occurred.
            IMG_ERR_UNSUPPORTED : The specified operation cannot be executed, since the imager is not integrated in the terminal.
         * 
            IMG_ERR_NOTINITIALIZED : IMGInit function is not executed.
            IMG_ERR_NOTCONNECTED : IMGConnect function is not executed.
            IMG_ERR_ENGINEBUSY : Error has occurred due to the busy state of the imager device.
            IMG_ERR_PARAMETER : An error has occurred due to an illegal parameter.
            IMG_ERR_NOTRIGGER : This function is terminated by the user function, or the decoding is terminated by IMGStopDecode function.
            IMG_ERR_NODECODE : Terminated without decoding.
            IMG_ERR_NOIMAGE : No valid image data can be returned.
         * 
            IMG_ERR_DRIVER : An error has occurred in the imager driver.
         * 
            IMG_ERR_NORESPONSE : No response from the device driver.
            IMG_ERR_BADREGION : The specified coordinate is out of the allowable range.
         */

        public enum decodeMode : int
        {///NOT CONFIRMED
            IMG_DECODEMODE_NORMAL, 
            IMG_DECODEMODE_MULTISTEP, 
            IMG_DECODEMODE_PACKAGE 
        }

        public enum scanMode : int
        {///NOT CONFIRMED
            IMG_SCAN_MODE_OUTDOOR,
            IMG_SCAN_MODE_WINDOWSIDE,
            IMG_SCAN_MODE_INDOOR,
            IMG_SCAN_MODE_WAREHOUSE
        }

        public enum ledMode : int
        {///NOT CONFIRMED
            IMG_LEDON,
            IMG_LEDOFF,
            IMG_LEDEROF
        }

        public enum buzzerMode : int
        {///NOT CONFIRMED
            IMG_BUZON,
            IMG_BUZOFF
        }

        public enum deliberationMode : int
        {///NOT CONFIRMED
            IMG_DECODE_VERYQUICK,
            IMG_DECODE_QUICK,
            IMG_DECODE_NORMAL,
            IMG_DECODE_DELIBERATE,
            IMG_DECODE_VERYDELIBERATE
        }

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGInit();

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGDeinit();

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGConnect();

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGDisconnect();

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetDecodeMode(ref Int32 pMode, ref Int32 pNum, string ptcSeparator);
        //pMode - decodeMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetDecodeMode(Int32 dwMode, Int32 dwNum, string tcSeparator);
 
        [DllImport(CMOS_LIB)]
        public static extern unsafe Int32 IMGWaitForDecode(Int32 dwTime, StringBuilder pMessage, StringBuilder pCodeID, StringBuilder pAimID, StringBuilder pSymModifier, Int32* pLength, IntPtr fpCallBack);

        public static unsafe Int32 IMGWaitForDecode(Int32 dwTime, out string pMessage, out string pCodeID, out string pAimID, out string pSymModifier, out int pLength, IntPtr fpCallBack)
        {
            StringBuilder _pMessage = new StringBuilder(), _pCodeID = new StringBuilder(), _pAimID = new StringBuilder(), _pSymModifier = new StringBuilder();
            Int32 res;
            fixed (int* ptr = &pLength)
                res = CMOSLib.IMGWaitForDecode(dwTime, _pMessage, _pCodeID, _pAimID, _pSymModifier, ptr, fpCallBack);
            pMessage = _pMessage.ToString();
            pCodeID = _pCodeID.ToString();
            pAimID = _pAimID.ToString();
            pSymModifier = _pSymModifier.ToString();
            return res;
        }

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGWaitForDecodeRaw(Int32 dwTime, ref byte pMessage, ref string pCodeID, ref string pAimID, ref string pSymModifier, ref Int32 pLength, IntPtr fpCallBack);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGStopDecode();

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetCode128(bool bEnabled, Int32 dwMinLength, Int32 dwMaxLength);

        [DllImport(CMOS_LIB)]
        private unsafe static extern Int32 IMGGetImage(byte* pImageBuffer, ref Int32 pSize, Int32 nLeft, Int32 nTop, Int32 nRight, Int32 nBottom, Int32 dwSkip, Int32 dwFormat, Int32 dwWhiteValue);

        public static Int32 IMGGetImage(out byte[] pImageBuffer, int x, int y, ImageFormat dwFormat, Int32 dwWhiteValue)
        {
            if (x > 640 || y > 480 || x<1 || y<1) throw new FormatException("Wrong picture size");
            int pSize = 0;
            pImageBuffer = new byte[x * y];
            unsafe
            {
                fixed (byte* iptr = pImageBuffer)
                {
                    return IMGGetImage(iptr, ref pSize, 0, 0, x, y, 1, (int)dwFormat, dwWhiteValue);//FIXME support lower resolution
                }
            }
        }
        
        [DllImport(CMOS_LIB)]
        private static extern Int32 IMGStartStream(Int32 nLeft, Int32 nTop, Int32 nRight, Int32 nBottom, Int32 dwSkip, Int32 dwFormat);

        public static Int32 IMGStartStream(int x, int y, ImageFormat dwFormat)
        {
            //if (x > 640 || y > 480 || x < 1 || y < 1) throw new FormatException("Wrong picture size");
            stream_x = x; stream_y = y;
            return IMGStartStream(0, 0, x, y, 1, (int)dwFormat);
        }

        [DllImport(CMOS_LIB)]
        private unsafe static extern Int32 IMGGetStreamData(byte* pImageBuffer, ref Int32 pSize);

        public static Int32 IMGGetStreamData(out byte[] pImageBuffer)
        {
            int pSize = 0;
            pImageBuffer = new byte[stream_x*stream_y];
            unsafe
            {
                fixed (byte* iptr = pImageBuffer)
                {
                    return IMGGetStreamData(iptr, ref pSize);
                }
            }
        }

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGStopStream();

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGCaptureSign(ref byte pImageBuffer, Int32 dwAspectRatio, Int32 nOffsetX, Int32 nOffsetY, UInt32 dwWidth, UInt32 dwHeight, Int32 nResolution, Int32 dwFormat);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGAimerOn(bool bEnable);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGIlluminationOn(bool bEnable);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetAimer(ref Int32 pAim);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetAimer(Int32 dwAim);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetIllumination(ref Int32 pIll);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetIllumination(Int32 dwIll);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetScanMode(ref Int32 pScanMode);
        //pScanMode - scanMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetScanMode(Int32 dwScanMode);
        //pScanMode - scanMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetImagerAPO(ref Int32 pTimeOut);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetImagerAPO(Int32 dwTimeOut);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetPrintWeight(ref Int32 pPrintWeight);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetPrintWeight(Int32 dwPrintWeight);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetLED(ref Int32 pLED);
        //pLED - ledMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetLED(Int32 dwLED);
        //dwLED - ledMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetBuzzer(ref Int32 pBuzzer);
        //pBuzzer - buzzerMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetBuzzer(Int32 dwBuzzer);
        //dwBuzzer - buzzerMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGGetDeliberation(ref Int32 pDeliberateTime);
        //pDeliberateTime - deliberationMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSetDeliberation(Int32 dwDeliberateTime);
        //dwDeliberateTime - deliberationMode
        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGLoadConfigFile(string pFileName);

        [DllImport(CMOS_LIB)]
        public static extern Int32 IMGSaveConfigFile(string pFileName);
    }
}