using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
    public enum DataTypes
    {
        Null,
        String,
        Image
    }
    public class MsgData
    {
        public MsgData (Image ImgData, string StrData)
        {
            imgData = ImgData;
            strData = StrData;
            ImageRecource = null; ;

        }

        public ImageSource ImageRecource
        {
            get
            {
                if (imgData != null)
                {
                    return Imaging.CreateBitmapSourceFromHBitmap((new Bitmap(this.imgData)).GetHbitmap(),
                                                                                            IntPtr.Zero,
                                                                                            Int32Rect.Empty,
                                                                                            BitmapSizeOptions.FromEmptyOptions());
                }
                else { return null; }
            }
            set 
            {
                
            
            }
        }
       
        
        public Image imgData { get; set; }

        public string strData { get; set; }
    }
    public class Controller
    {
        
        public static byte[] Coding(dynamic MsgData, DataTypes type)
        {
            byte[] ReturnBytes;
           
            switch (type)
            {
                case (DataTypes.String):
                    ReturnBytes = new byte[1025];
                    int NumberOfBytes;
                    NumberOfBytes = Encoding.UTF8.GetBytes(MsgData, 0, MsgData.Length,ReturnBytes,1);
                   
                    ReturnBytes[0] = Convert.ToByte(type);
        
                    return ReturnBytes;

                case (DataTypes.Image):
                    using (var ms = new System.IO.MemoryStream())
                    {
                        MsgData.Save(ms, MsgData.RawFormat);
                        ms.Position = 0;
                        using(BinaryReader br=new BinaryReader(ms))
                        {
                            ReturnBytes = new byte[br.BaseStream.Length + 1];
                            for(int i=1;i< br.BaseStream.Length+1;i++)
                            {
                                ReturnBytes[i] = br.ReadByte();
                            }
                            ReturnBytes[0] = Convert.ToByte(DataTypes.Image);
                            return ReturnBytes;
                        }
                        
                    }
                default:
                    return null;
            }
        }
        
       public static MsgData Decoder (byte[] BiteContent)
        {
            MsgData NewMessage = new MsgData(null, null);
            switch(Convert.ToInt32(BiteContent[0]))
            {
                case (2):  //Image
                    /*ImageConverter imgCon = new ImageConverter();
                    return (byte[])imgCon.ConvertTo(MsgData, typeof(byte[]));*/
                    MemoryStream memoryStream = new MemoryStream();

                    using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                    {
                        for (int i = 1; i < BiteContent.Length; i++)
                        {
                            binaryWriter.BaseStream.WriteByte(BiteContent[i]);
                        }

                        binaryWriter.BaseStream.Position = 0;
                        NewMessage.imgData = Image.FromStream(binaryWriter.BaseStream);
                        NewMessage.ImageRecource = Imaging.CreateBitmapSourceFromHBitmap((new Bitmap(memoryStream)).GetHbitmap(),
                                                                                        IntPtr.Zero,
                                                                                        Int32Rect.Empty,
                                                                                        BitmapSizeOptions.FromEmptyOptions());
                    }
                    
                    break;
                case (1):  //String
                    string test = Encoding.UTF8.GetString(BiteContent,1,BiteContent.Length-1);
                    NewMessage.strData = test;
                    break;
                default:    //Null
                    break;
            }
            
            return NewMessage;
        }
        
    }
}
