using System;
using System.Collections.Generic;
using System.Text;

namespace kkbot.DS.SMHI
{

  public class SMHIBotData
  {
    public string Filename { get; set; }
    public string SmhiUri { get; set; }
    public string OutputPreString { get; set; }
    public string OutputPostString { get; set; }
    public string FinalString { get; set; }

    // Parameterless constructor
    public SMHIBotData() { }

    // Optional: Keep your existing parameterized constructor if you need it
    public SMHIBotData(string filename, string smhiUri, string preString, string postString)
    {
      Filename = filename;
      SmhiUri = smhiUri;
      OutputPreString = preString;
      OutputPostString = postString;
    }
  }


  //public class SMHIBotData
  //{

  //  public string filename;
  //  public string smhiUri;
  //  public string outputPreString;
  //  public string outputPostString;
  //  public string finalString;

  //  public SMHIBotData(string _filename, string _smhiuri, string _prestring, string _poststring)
  //  {
  //    filename = _filename;
  //    smhiUri = _smhiuri;
  //    outputPreString = _prestring;
  //    outputPostString = _poststring;
  //  }


  //}
}
