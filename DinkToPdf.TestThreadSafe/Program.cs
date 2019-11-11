using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace DinkToPdf.TestThreadSafe
{
    public class Program
    {
        static SynchronizedConverter _converter;

        public static void Main(string[] args)
        {
            _converter = new SynchronizedConverter(new PdfTools());
            
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 },
                },
                Objects = {
                    new ObjectSettings() {
                        Page = "http://www.color-hex.com/"
                    }
                }
            };

            Task.Run(() => Action(doc));
            
            var doc2 = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4Small
                },

                Objects = {
                    new ObjectSettings()
                    {
                        Page = "http://google.com/",
                    }
                }
            };


            Task.Run(() => Action(doc2));

            Console.ReadKey();
        }

        private static void Action(IDocument doc)
        {
            byte[] pdf = _converter.Convert(doc);

            if (!Directory.Exists("Files"))
            {
                Directory.CreateDirectory("Files");
            }

            using FileStream stream = new FileStream(@"Files\" + DateTime.UtcNow.Ticks + ".pdf", FileMode.Create);
            stream.Write(pdf, 0, pdf.Length);
        }
    }
}
