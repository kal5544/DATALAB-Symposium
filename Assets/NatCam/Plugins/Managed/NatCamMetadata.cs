/*
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//Built upon ZXing.NET 0.14.0.0

#define NATCAM_ZXING_DEPENDENCY

using UnityEngine;
using System.Collections.Generic;

#if NATCAM_ZXING_DEPENDENCY
using ZXing;
#endif

namespace NatCamU {
    
    namespace Internals {
        
        public static class NatCamMetadata {

			public static void Decode (Color32[] frame, int width, int height, NatCamDispatch dispatch, BarcodeCallback callback) {
				if (callback == null) return;
				#if NATCAM_ZXING_DEPENDENCY
				results = reader.DecodeMultiple(frame, width, height);
				dispatch.Dispatch(() => results.ForEach(result => callback(NatCamBarcode(result))));
				#endif
			}

            #region --Ctor--

            #if NATCAM_ZXING_DEPENDENCY
            static BarcodeReader reader;
            static Result[] results;
            static NatCamMetadata () {
                reader = new BarcodeReader();
                //reader.Options.TryHarder =
                reader.TryInverted =
                reader.AutoRotate = true;
            }
            #endif
            #endregion


            #region --Util--

            #if NATCAM_ZXING_DEPENDENCY
            private static Barcode NatCamBarcode (Result result) { //We don't want hard ZXing.NET dependency, so extension instead of operator
                return new Barcode(result.Text, NatCamBarcodeFormat(result.BarcodeFormat));
            }

            private static BarcodeFormat NatCamBarcodeFormat (ZXing.BarcodeFormat format) {
                switch (format) {
                    case ZXing.BarcodeFormat.QR_CODE : return BarcodeFormat.QR;
                    case ZXing.BarcodeFormat.EAN_13 : return BarcodeFormat.EAN_13;
                    case ZXing.BarcodeFormat.EAN_8 : return BarcodeFormat.EAN_8;
                    case ZXing.BarcodeFormat.DATA_MATRIX : return BarcodeFormat.DATA_MATRIX;
                    case ZXing.BarcodeFormat.PDF_417 : return BarcodeFormat.PDF_417;
                    case ZXing.BarcodeFormat.CODE_128 : return BarcodeFormat.CODE_128;
                    case ZXing.BarcodeFormat.CODE_93 : return BarcodeFormat.CODE_93;
                    case ZXing.BarcodeFormat.CODE_39 : return BarcodeFormat.CODE_39;
                    default : return BarcodeFormat.ALL;
                }
            }
            #endif

            private static void ForEach<T> (this T[] array, System.Action<T> action) {
                if (array == null) return;
                for (int i = 0, len = array.Length; i < len; i++) action(array[i]);
            }
            #endregion
        }
    }
}