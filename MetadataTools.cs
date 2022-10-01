using System.Net;
using System.Text.RegularExpressions;
using System;

namespace Speech_Synthesizer
{
    internal class MetadataTools
    {
        private void addcoverart(string audiopath, string picturepath)
        {
            TagLib.File TagLibFile = TagLib.File.Create(audiopath);
            TagLib.Picture picture = new TagLib.Picture(picturepath);
            TagLib.Id3v2.AttachedPictureFrame albumCoverPictFrame = new TagLib.Id3v2.AttachedPictureFrame(picture);
            albumCoverPictFrame.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            albumCoverPictFrame.Type = TagLib.PictureType.FrontCover;
            TagLib.IPicture[] pictFrames = new TagLib.IPicture[1];
            pictFrames[0] = (TagLib.IPicture)albumCoverPictFrame;
            TagLibFile.Tag.Pictures = pictFrames;
            TagLibFile.Save();
        }
    }
}
