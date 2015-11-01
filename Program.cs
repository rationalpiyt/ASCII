//Based off code from Thinathayalan Ganeson, his blog is
//http://CyberSannyasi.blogspot.com

//Turning Applications into ASCII Art
//If creating a new project, add the references for  System.Drawing and System.Windows.Forms

//Video 1: What is ASCII Art and pulling images into C#
//Video 2: What is a bitmap and how can you find out individual pixel information
//Video 3: AspectRatio and transforming the image
//Video 4: Form application and web browser within forms
//Video 5: Improvements and color

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

class Program {
    public static class Globals {
        //C# does not have native Global Variables.
        public static string content;
        public static string[] ascii = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", "&nbsp;" };
    }

    public static Bitmap AsciiAspectRatio(Bitmap image, int h, int w) {
        //This method preserves the aspect ration based on the desired width of the ASCII Art given as w.
        h = (int)Math.Ceiling((double)image.Height * w / image.Width);

        //Generate a new bitmap template.
        Bitmap result = new Bitmap(w, h);
        Graphics g = Graphics.FromImage((Image)result);
        //InterpolationMode generates a more high quality graphic.
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.DrawImage(image, 0, 0, w, h);
        g.Dispose();
        return result;
    }

    private static string ConvertToAscii(Bitmap image) {
        Boolean endLine = false;
        StringBuilder sb = new StringBuilder();

        //For each pixel in the picture, find its color value.
        for (int h = 0; h < image.Height; h++) {
            for (int w = 0; w < image.Width; w++) {
                Color pixelColor = image.GetPixel(w, h);
                //Divide the color value by three for normalization.
                int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                //Find the grayscale value.
                Color gray = Color.FromArgb(red, green, blue);

                if (!endLine) {
                    //Comparing the grayscale value to the red value of the pixel,
                    //Add the appropriate ASCII character to the stringBuilder.
                    int index = (gray.R * 10) / 255;
                    sb.Append(Globals.ascii[index]);
                }
            }
            if (!endLine) {
                //As we are going to build the new picture in the form browser
                //Adding <BR> will signify a line break.
                sb.Append("<BR>");
                endLine = true;
            }
            else
                endLine = false;
        }
        return sb.ToString();
    }

    private static void FormApplication()
    {
        //Forms allow for a cleaner looking output window rather than the
        //command prompt looking window.
        Form newForm = new Form();
        WebBrowser browser = new System.Windows.Forms.WebBrowser();                                         //Browser within form
        browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top      //Anchors the browser to the sides of the form window
                        | System.Windows.Forms.AnchorStyles.Bottom) 
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
        browser.Location = new System.Drawing.Point(0, 0);                                                  //Browser appears at the top left corner of the form window
        browser.Name = "Ascii Art";
        browser.Size = new System.Drawing.Size(1000, 1000);
        newForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;                                    //Does not scale the characters above their current size
        newForm.ClientSize = new System.Drawing.Size(1000, 1000);                                           //This allows all characters to take the same space.
        newForm.Controls.Add(browser);
        newForm.PerformLayout();
        browser.DocumentText = "<pre>" + "<Font size=0>" + Globals.content + "</Font></pre>";               //Adds the picture to the form window browser.
        Application.Run(newForm);
    }
   
    [STAThread] //This allows the browser and form to be built from this Main sequence.
    public static void Main(string[] args)
    {
        OpenFileDialog file = new OpenFileDialog();                     //Choosing the file we wish to manipulate.
        DialogResult dr = file.ShowDialog();
        string filePath = file.FileName;
        Bitmap image = new Bitmap(filePath);

        int asciiWidth = 310;                                           //We will base the aspect ratio on our desired width.
        int asciiHeight = 0;
        image = AsciiAspectRatio(image, asciiHeight, asciiWidth);

        Globals.content = ConvertToAscii(image);

        FormApplication();
    }
}
