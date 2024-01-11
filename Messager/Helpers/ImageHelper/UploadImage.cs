using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Helpers.ImageHelper
{
    public class UploadImage
    {
        public static async Task<FileResult> OpenMediaPickerAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please a pick photo"
                });

                if (result.ContentType == "image/png" ||
                    result.ContentType == "image/jpeg" ||
                    result.ContentType == "image/jpg")
                    return result;
                else
                    await Application.Current.MainPage.DisplayAlert("Error Type Image", "Please choose a new image", "Ok");

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        
        public static async Task<Stream> FileResultToStream(FileResult fileResult)
        {
            if (fileResult == null)
                return null;

            return await fileResult.OpenReadAsync();
        }

     
        public static Stream ByteArrayToStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
        public static byte[] StreamToByte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string ByteBase64ToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

      
        public static byte[] StringToByteBase64(string text)
        {
            return Convert.FromBase64String(text);
        }


        public static async Task<ImageFile> Upload(FileResult fileResult)
        {
            byte[] bytes;

            try
            {
                using (var ms = new MemoryStream())
                {
                    var stream = await FileResultToStream(fileResult);
                    stream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                return new ImageFile
                {
                    byteBase64 = ByteBase64ToString(bytes),
                    ContentType = fileResult.ContentType,
                    FileName = fileResult.FileName
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
