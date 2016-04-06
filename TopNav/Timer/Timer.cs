using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Meetings;
using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace TopNav.Timer
{
    public class Timer : WebPart
    {
        public enum TimerUnits
        {
            full,
            hours,
            days
        }

        public enum FilterTarget
        {
            StartTime,
            EndTime
        }

        protected DateTime _StartDate = DateTime.Now;

        protected DateTime _EndDate = DateTime.Now.AddHours(1.0);

        protected bool _ShowStartEndTime;

        protected bool _AmPm;

        protected bool _ShowRemainingTime = true;

        protected bool _ShowElapsedTime;

        protected bool _SuppressSeconds;

        protected bool _ShowIcon;

        protected Timer.TimerUnits _Unit;

        protected Timer.FilterTarget _FilterField = Timer.FilterTarget.EndTime;

        protected string _TimerStyle = "color:white;background-color:rgb(51,103,153);padding:10px;text-align:center";

        protected string _TimeFont = "17pt Segoe UI;color:white;vertical-align:bottom";

        protected string _CountdownCaption = "Count Down to Census 2020:";

        protected string _ElapsedCaption = "Elapsed Time:";

        protected string _TimeoutMessage = "";

        protected string _Header = "";

        protected string _Link = "";

        protected string _Localization = "d;h";

        protected string _Options = "";

        private static int TimerInstanceCounter;

        private IWebPartField myProvider;

        private string myField = "";

        [Personalizable, WebBrowsable(true), WebDescription("Countdown start time (Format: mm/dd/yyyy hh:mm:ss)"), WebDisplayName("Start time")]
        public DateTime StartDate
        {
            get
            {
                return this._StartDate;
            }
            set
            {
                this._StartDate = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Countdown end time (Format: mm/dd/yyyy hh:mm:ss)"), WebDisplayName("End time")]
        public DateTime EndDate
        {
            get
            {
                return this._EndDate;
            }
            set
            {
                this._EndDate = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("either display or hide the timer start and the end time"), WebDisplayName("Display Start and End Time")]
        public bool ShowStartEndTime
        {
            get
            {
                return this._ShowStartEndTime;
            }
            set
            {
                this._ShowStartEndTime = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("switch from military to AM/PM time format"), WebDisplayName("Use AM/PM time format")]
        public bool AmPm
        {
            get
            {
                return this._AmPm;
            }
            set
            {
                this._AmPm = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("either display or hide the remaining time"), WebDisplayName("Display Remaining Time")]
        public bool ShowRemainingTime
        {
            get
            {
                return this._ShowRemainingTime;
            }
            set
            {
                this._ShowRemainingTime = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("either display or hide the elapsed time"), WebDisplayName("Display Elapsed Time")]
        public bool ShowElapsedTime
        {
            get
            {
                return this._ShowElapsedTime;
            }
            set
            {
                this._ShowElapsedTime = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("either display or hide the seconds in the remaining and elapsed timer"), WebDisplayName("Suppress Seconds")]
        public bool SuppressSeconds
        {
            get
            {
                return this._SuppressSeconds;
            }
            set
            {
                this._SuppressSeconds = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("either display or hide the timer icon"), WebDisplayName("Display Timer Icon")]
        public bool ShowIcon
        {
            get
            {
                return this._ShowIcon;
            }
            set
            {
                this._ShowIcon = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Select full timer (d:h:m:s), hours or days display"), WebDisplayName("Display Units")]
        public Timer.TimerUnits Unit
        {
            get
            {
                return this._Unit;
            }
            set
            {
                this._Unit = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("choose the field to connect the Filter value to if using a Filter web part connection"), WebDisplayName("Connect Filter to")]
        public Timer.FilterTarget FilterField
        {
            get
            {
                return this._FilterField;
            }
            set
            {
                this._FilterField = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter the CSS style of the web part"), WebDisplayName("Timer CSS Style")]
        public string TimerStyle
        {
            get
            {
                return this._TimerStyle;
            }
            set
            {
                this._TimerStyle = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter the font of time display in points (CSS)"), WebDisplayName("Time Font")]
        public string TimeFont
        {
            get
            {
                return this._TimeFont;
            }
            set
            {
                this._TimeFont = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter the label for the remaining time display"), WebDisplayName("Time Remaining Label")]
        public string CountdownCaption
        {
            get
            {
                return this._CountdownCaption;
            }
            set
            {
                this._CountdownCaption = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter the label for the elapsed time display"), WebDisplayName("Elapsed Time Label")]
        public string ElapsedCaption
        {
            get
            {
                return this._ElapsedCaption;
            }
            set
            {
                this._ElapsedCaption = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter the optional text to be displayed when the timeout has been reached"), WebDisplayName("Timeout Message")]
        public string TimeoutMessage
        {
            get
            {
                return this._TimeoutMessage;
            }
            set
            {
                this._TimeoutMessage = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("optionally display a custom web part header"), WebDisplayName("Header Text")]
        public string Header
        {
            get
            {
                return this._Header;
            }
            set
            {
                this._Header = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("optionally link the web part to an URL"), WebDisplayName("Redirection Link")]
        public string Link
        {
            get
            {
                return this._Link;
            }
            set
            {
                this._Link = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter the localized names for 'days' and 'hours'"), WebDisplayName("Localization")]
        public string Localization
        {
            get
            {
                return this._Localization;
            }
            set
            {
                this._Localization = value;
            }
        }

        [Personalizable, WebBrowsable(true), WebDescription("Enter additional web part options as needed"), WebDisplayName("Options")]
        public string Options
        {
            get
            {
                return this._Options;
            }
            set
            {
                this._Options = value;
            }
        }



        private void ReceiveField(object objField)
        {
            if (objField != null)
            {
                try
                {
                    this.myField = (string)objField;
                }
                catch
                {
                    this.myField = "";
                }
            }
        }

        [ConnectionConsumer("Field")]
        public void SetConnectionInterface(IWebPartField provider)
        {
            this.myProvider = provider;
            provider.GetFieldValue(new FieldCallback(this.ReceiveField));
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (HttpContext.Current.Request.QueryString["AEtrace"] == "1")
            {
                return;
            }
            Timer.TimerInstanceCounter++;
            string newValue = "Aeti" + Timer.TimerInstanceCounter % 99;
            string text = "";
            string text2 = "";
            bool flag = false;
            int major = SPFarm.Local.BuildVersion.Major;
            string text3 = "";
            int num = -1;
            string text4 = "";
            string text5 = "";
            string text6 = "";
            string text7 = "m";
            string text8 = "s";
            string[] array = this._Options.Split(new char[]
			{
				'|'
			});
            string text11;
            for (int i = 0; i < array.Length; i++)
            {
                string text9 = array[i];
                string text10 = "";
                int num2 = text9.IndexOf("=");
                if (num2 > 0)
                {
                    text10 = text9.Substring(num2 + 1);
                    text9 = text9.Substring(0, num2);
                }
                switch (text11 = text9.ToLower())
                {
                    case "weekday":
                        int.TryParse(text10, out num);
                        break;
                    case "orange":
                        text5 = text10;
                        break;
                    case "red":
                        text4 = text10;
                        break;
                    case "footer":
                        text6 = text10;
                        break;
                    case "minutes":
                        text7 = text10;
                        break;
                    case "seconds":
                        text8 = text10;
                        break;
                    case "css":
                        text3 = text10;
                        break;
                    case "trace":
                        flag = (text10 == "1");
                        break;
                }
                text11 = text2;
                text2 = string.Concat(new string[]
				{
					text11,
					"<li>Option ",
					text9,
					"=",
					text10
				});
            }
            string str = "/_layouts/images/TimerWebpart/";
            if (major == 15)
            {
                str = "/_layouts/15/images/TimerWebpart/";
            }
            DateTime now = DateTime.Now;
            DateTime value = DateTime.Now;
            try
            {
                value = this._StartDate;
                if (!string.IsNullOrEmpty(this.myField) && this._FilterField == Timer.FilterTarget.StartTime)
                {
                    DateTime dateTime;
                    DateTime.TryParse(this.myField, out dateTime);
                    value = dateTime;
                }
                if (value.Day == 1 && value.Month == 1 && value.Year == 2000)
                {
                    value = new DateTime(now.Year, now.Month, now.Day, value.Hour, value.Minute, value.Second);
                }
                text2 = text2 + "<li>Start Date=" + value.ToString();
            }
            catch (Exception ex)
            {
                text = text + "<br>StartDate Error: " + ex.Message;
                text2 = text2 + "<li>" + ex.StackTrace;
            }
            DateTime dateTime2 = DateTime.Now;
            string text12 = this._TimerStyle;
            string text13 = "";
            object obj;
            try
            {
                dateTime2 = this._EndDate;
                if (!string.IsNullOrEmpty(this.myField) && this._FilterField == Timer.FilterTarget.EndTime)
                {
                    DateTime dateTime3;
                    DateTime.TryParse(this.myField, out dateTime3);
                    dateTime2 = dateTime3;
                }
                if (dateTime2.Day == 1 && dateTime2.Month == 1 && dateTime2.Year == 2000)
                {
                    if (num >= 0)
                    {
                        int dayOfWeek = (int)now.DayOfWeek;
                        int num4 = num - dayOfWeek;
                        if (num4 < 0)
                        {
                            num4 += 7;
                        }
                        obj = text2;
                        text2 = string.Concat(new object[]
						{
							obj,
							"<li>Weekday=",
							dayOfWeek,
							"|",
							num,
							"|Delta=",
							num4
						});
                        dateTime2 = new DateTime(now.Year, now.Month, now.Day, dateTime2.Hour, dateTime2.Minute, dateTime2.Second);
                        dateTime2 = dateTime2.AddDays((double)num4);
                    }
                    else
                    {
                        dateTime2 = new DateTime(now.Year, now.Month, now.Day, dateTime2.Hour, dateTime2.Minute, dateTime2.Second);
                    }
                }
                text2 = text2 + "<li>End Date=" + dateTime2.ToString();
            }
            catch (Exception ex2)
            {
                text = text + "<br>EndDate Error: " + ex2.Message;
                text2 = text2 + "<li>" + ex2.StackTrace;
            }
            bool flag2 = false;
            if (!string.IsNullOrEmpty(text4))
            {
                string[] array2 = text4.Split(new char[]
				{
					','
				});
                int num5 = 0;
                int.TryParse(array2[0], out num5);
                flag2 = (now > dateTime2.AddDays((double)(-(double)num5)));
                if (flag2)
                {
                    text2 = text2 + "<li>Red Phase=" + num5;
                    text2 = text2 + "<br>TOD=" + now.ToString();
                    text2 = text2 + "<br>RED=" + dateTime2.AddDays((double)(-(double)num5)).ToString();
                    if (array2.Length > 1)
                    {
                        text12 = text12 + ";" + array2[1];
                    }
                    if (array2.Length > 2)
                    {
                        text13 = array2[2];
                    }
                }
            }
            if (!string.IsNullOrEmpty(text5) && !flag2)
            {
                string[] array3 = text5.Split(new char[]
				{
					','
				});
                int num6 = 0;
                int.TryParse(array3[0], out num6);
                if (now > dateTime2.AddDays((double)(-(double)num6)))
                {
                    text2 = text2 + "<li>orange Phase=" + num6;
                    if (array3.Length > 1)
                    {
                        text12 = text12 + ";" + array3[1];
                    }
                    if (array3.Length > 2)
                    {
                        text13 = array3[2];
                    }
                }
            }
            SPSite arg_614_0 = SPContext.Current.Site;
            SPWeb web = SPContext.Current.Web;
            if (SPMeeting.IsMeetingWorkspaceWeb(web))
            {
                SPList sPList = web.Lists["Meeting Series"];
                SPListItemCollection items = sPList.Items;
                foreach (SPListItem sPListItem in items)
                {
                    int num7 = (int)sPListItem["InstanceID"];
                    if (num7 > 0)
                    {
                        bool flag3 = (bool)sPListItem["fRecurrence"];
                        if (flag3)
                        {
                            SPListItem sPListItem2 = sPList.Items[0];
                            string arg_6B4_0 = (string)sPListItem2["EventUID"];
                        }
                        else
                        {
                            string arg_6C8_0 = (string)sPListItem["EventUID"];
                        }
                        value = (DateTime)sPListItem["EventDate"];
                        dateTime2 = (DateTime)sPListItem["EndDate"];
                    }
                }
            }
            string text14 = "text-align:center; color:#11AA11; font:" + this._TimeFont;
            string text15 = "text-align:center; color:#555599; font:" + this._TimeFont;
            string text16 = "";
            if (!string.IsNullOrEmpty(this._Link))
            {
                string text17 = this._Link;
                string str2 = "";
                int num8 = text17.IndexOf("|");
                if (num8 > 0)
                {
                    str2 = " title='" + text17.Substring(num8 + 1) + "'";
                    text17 = text17.Substring(0, num8);
                }
                text16 = " onclick='window.location.href=\"" + text17 + "\"'" + str2;
                text12 += ";cursor:pointer";
            }
            text11 = text;
            text = string.Concat(new string[]
			{
				text11,
				"<div style='",
				text12,
				"'",
				text16,
				">"
			});
            if (!string.IsNullOrEmpty(this._Header))
            {
                text += this._Header;
            }
            if (this._ShowIcon)
            {
                text = text + "<img style='float:left' src='" + str + "timer.png' />";
            }
            string text18 = this._Localization;
            if (string.IsNullOrEmpty(text18))
            {
                text18 = "d;h";
            }
            string[] array4 = text18.Split(new char[]
			{
				';'
			});
            string str3 = array4[0];
            string str4 = array4[1];
            string str5 = array4[0];
            string str6 = array4[1];
            if (array4.Length > 2)
            {
                str5 = array4[2];
            }
            if (array4.Length > 3)
            {
                str6 = array4[3];
            }
            string text19 = "Start";
            if (array4.Length > 4)
            {
                text19 = array4[4];
            }
            string text20 = "End";
            if (array4.Length > 5)
            {
                text20 = array4[5];
            }
            if (this._ShowStartEndTime)
            {
                string str7 = "HH";
                if (this._AmPm)
                {
                    str7 = "hh";
                }
                text += "<table border=0 cellpadding=1 cellspacing=0>";
                text11 = text;
                text = string.Concat(new string[]
				{
					text11,
					"<tr><td align=right>",
					text19,
					":</td><td>",
					value.ToString("yyyy-MM-dd " + str7 + ":mm tt"),
					"</td></tr>"
				});
                text11 = text;
                text = string.Concat(new string[]
				{
					text11,
					"<tr><td align=right>",
					text20,
					":</td><td>",
					dateTime2.ToString("yyyy-MM-dd " + str7 + ":mm tt"),
					"</td></tr>"
				});
                text += "</table><br/>";
            }
            TimeSpan timeSpan = dateTime2.Subtract(now);
            TimeSpan timeSpan2 = now.Subtract(value);
            TimeSpan timeSpan3 = dateTime2.Subtract(value);
            double arg_9C4_0 = timeSpan3.TotalMinutes;
            double totalSeconds = timeSpan3.TotalSeconds;
            string text21 = "<br/>";
            string text22 = this._CountdownCaption;
            if (text22.EndsWith("~"))
            {
                text21 = "&nbsp;";
                text22 = text22.Replace("~", "");
            }
            if (this._ShowRemainingTime)
            {
                text11 = text;
                text = string.Concat(new string[]
				{
					text11,
					text22,
					text21,
					"<span style='",
					text14,
					"' id=AECountdownTimer??></span><br/><br/>"
				});
            }
            if (this._ShowElapsedTime)
            {
                text11 = text;
                text = string.Concat(new string[]
				{
					text11,
					"<span style='color:555599'>",
					this._ElapsedCaption,
					"</span>",
					text21,
					"<span style='",
					text15,
					"' id=AEElapsedTimer??></span><br/>"
				});
            }
            if (!string.IsNullOrEmpty(text13))
            {
                text += text13;
            }
            if (!string.IsNullOrEmpty(text6))
            {
                text += text6;
            }
            string str8 = "<span style='color:red'>0</span>";
            if (!string.IsNullOrEmpty(this._TimeoutMessage))
            {
                str8 = "<span style='color:black;font-size:13px'>" + this._TimeoutMessage + "</span>";
            }
            text += "</div>";
            text += "<script language=\"javascript\">";
            text += "function displayTimer??(countdown,countup) {\n";
            text += "  window.remaining?? = parseFloat(countdown);\n";
            text += "  window.elapsed?? = parseFloat(countup);\n";
            text += "  mytimer=setTimeout('displayCounter??()',1000)\n";
            text += "}\n";
            text += "function displayCounter??() {\n";
            text += "var remain = 0;\n";
            text += "if (window.elapsed??>0) {\n";
            text += "  remain = window.remaining??;\n";
            text += "}\n";
            text += "else {\n";
            obj = text;
            text = string.Concat(new object[]
			{
				obj,
				"  remain = ",
				totalSeconds,
				";\n"
			});
            text += "}\n";
            if (this._ShowRemainingTime)
            {
                text += "if (window.remaining??>0) {\n";
                if (this._Unit == Timer.TimerUnits.days)
                {
                    text += "  var remainingDays = remain/86400;\n";
                    text += "  if (remainingDays >= 2) {\n";
                    text += "    remainingDays = remainingDays.toFixed(0);\n";
                    text += "  }\n";
                    text += "  else {\n";
                    text += "    remainingDays = remainingDays.toFixed(1);\n";
                    text += "  };\n";
                    text = text + "  document.getElementById('AECountdownTimer??').innerHTML = remainingDays + '" + str3 + "';\n";
                }
                else if (this._Unit == Timer.TimerUnits.hours)
                {
                    text = text + "  document.getElementById('AECountdownTimer??').innerHTML = (remain/3600).toFixed(1) + '" + str4 + "';\n";
                }
                else
                {
                    text += "  var years = Math.floor(remain / 86400);\n";
                    

                  /*  text += " var days=Math.floor(window.elapsed?? / 86400);\n";
                    text += "  var months = Math.floor(days / 28);\n";
                    text += "  s = Math.floor(remain / 86400);\n";
                    text += "  var hours = Math.floor((remain - (days * 86400 ))/3600)\n";
                    text += "  var minutes = Math.floor((remain - (days * 86400 ) - (hours *3600 ))/60)\n";
                    text += "  var secs = Math.floor((remain - (days * 86400 ) - (hours *3600 ) - (minutes*60)))\n";*/

                   text += " var secs = Math.floor(remain);\n ";
                   text += " var minutes = Math.floor(secs / 60);\n ";
                   text += " var hours = Math.floor(minutes / 60);\n";
                   text += " var days = Math.floor(hours / 24);\n";
                   text += " var months = Math.floor(days / 28);\n";

                   text += " days = days - (months * 28); \n";
                   text += "hours = hours - (days * 24) - (months *28 *24);\n";
                   text += "minutes = minutes - (days * 24 * 60) - (hours * 60) - (months * 28 * 24 * 60);\n";
                   text += "secs = secs - (days * 24 * 60 * 60) - (hours * 60 * 60) - (minutes * 60) - (months * 28 * 24 * 60 * 60);\n";

                    text += "  if (secs<10) secs='0' + secs;\n";
                    text += "  var shours = \"\";\n";
                    text = text + "  if (months==1) shours = months + 'm';\n";
                    text = text + "  if (months!=1 && months>0) shours = months + 'm ';\n";
                    text = text + "  if (days==1) shours = shours + days + '" + str5 + " ';\n";
                    text = text + "  if (days!=1 && days>0) shours = shours + days + '" + str3 + " ';\n";
                    text += "  if (hours>0 || days>0) {\n";
                    text = text + "    if (hours==1) {shours += hours + '" + str6 + " '}\n";
                    text = text + "    else {shours += hours + '" + str4 + " '};\n";
                    text += "  }\n";
                    if (this._SuppressSeconds)
                    {
                        text = text + "  document.getElementById('AECountdownTimer??').innerHTML = shours + minutes + '" + text7 + "';\n";
                    }
                    else
                    {
                        text11 = text;
                        text = string.Concat(new string[]
						{
							text11,
							"  document.getElementById('AECountdownTimer??').innerHTML = shours + minutes + '",
							text7,
							" ' + secs + '",
							text8,
							"';\n"
						});
                    }
                }
                text += "}\n";
                text += "else {\n";
                text = text + "  document.getElementById('AECountdownTimer??').innerHTML = \"" + str8 + "\";\n";
                text += "}\n";
            }
            if (this._ShowElapsedTime)
            {
                text += "if (window.elapsed??>0) {\n";
                if (this._Unit == Timer.TimerUnits.days)
                {
                    text += "  var elapsedDays = window.elapsed??/86400;\n";
                    text += "  if (elapsedDays >= 2) {\n";
                    text += "    elapsedDays = elapsedDays.toFixed(0);\n";
                    text += "  }\n";
                    text += "  else {\n";
                    text += "    elapsedDays = elapsedDays.toFixed(1);\n";
                    text += "  };\n";
                    text = text + "  document.getElementById('AEElapsedTimer??').innerHTML = elapsedDays + '" + str3 + "';\n";
                }
                else if (this._Unit == Timer.TimerUnits.hours)
                {
                    text = text + "  document.getElementById('AEElapsedTimer??').innerHTML = (window.elapsed??/3600).toFixed(1) + '" + str4 + "';\n";
                }
                else
                {
                    text += " var years =Math.floor(window.elapsed?? / 86400));\n";
                    
                   /* text += " var days=Math.floor(window.elapsed?? / 86400);\n";
                    text += " var months =Math.floor(days/28);\n";
                    text += " days = days-(months*28);";
                    text += " var hours = Math.floor((window.elapsed?? - (days * 86400 ))/3600)\n";
                    text += " var minutes = Math.floor((window.elapsed?? - (days * 86400 ) - (hours *3600 ))/60)\n";
                    text += " var secs = Math.floor((window.elapsed?? - (days * 86400 ) - (hours *3600 ) - (minutes*60)))\n";*/

                    text += " var secs = Math.floor(window.elapsed??);\n ";
                    text += " var minutes = Math.floor(window.elapsed?? / 60);\n ";
                    text += " var hours = Math.floor(minutes / 60);\n";
                    text += " var days = Math.floor(hours / 24);\n";
                    text += " var months = Math.floor(days / 28);\n";

                    text += " days = days - (months * 28); \n";
                    text += "hours = hours - (days * 24);\n";
                    text += "minutes = minutes - (days * 24 * 60) - (hours * 60);\n";
                    text += "secs = secs - (days * 24 * 60 * 60) - (hours * 60 * 60) - (minutes * 60);\n";

                    text += " if (secs<10) secs='0' + secs;\n";
                    text += " var shours = \"\";\n";
                    text = text + "  if (months==1) shours = months + 'm';\n";
                    text = text + "  if (months!=1 && months>0) shours = months + 'm ';\n";
                    text = text + " if (days==1) shours = shours + days + '" + str5 + " ';\n";
                    text = text + " if (days!=1 && days>0) shours = shours + days + '" + str3 + " ';\n";
                    text += "  if (hours>0 || days>0) {\n";
                    text = text + "    if (hours==1) {shours += hours + '" + str6 + " '}\n";
                    text = text + "    else {shours += hours + '" + str4 + " '};\n";
                    text += "  }\n";
                    if (this._SuppressSeconds)
                    {
                        text = text + " document.getElementById('AEElapsedTimer??').innerHTML = shours + minutes + '" + text7 + "';\n";
                    }
                    else
                    {
                        text11 = text;
                        text = string.Concat(new string[]
						{
							text11,
							" document.getElementById('AEElapsedTimer??').innerHTML = shours + minutes + '",
							text7,
							" ' + secs + '",
							text8,
							"';\n"
						});
                    }
                }
                text += "}\n";
                text += "else {\n";
                text += "  document.getElementById('AEElapsedTimer??').innerHTML = \"0\";\n";
                text += "}\n";
            }
            text += "if (window.remaining??>0) window.remaining?? = window.remaining?? - 1;\n";
            text += "window.elapsed?? = window.elapsed?? + 1;\n";
            text += "tt = displayTimer??(window.remaining??,window.elapsed??);\n";
            text += "}\n";
            text11 = text;
            text = string.Concat(new string[]
			{
				text11,
				"displayTimer??(",
				timeSpan.TotalSeconds.ToString("F0"),
				",",
				timeSpan2.TotalSeconds.ToString("F0"),
				");\n"
			});
            text += "</script>";
            writer.Write(text.Replace("??", newValue));
            if (!string.IsNullOrEmpty(text3))
            {
                writer.Write("<style>" + text3 + "</style>");
            }
            if (flag)
            {
                writer.Write("<ul style='font:8pt Consolas;color:black;background-color:#ffffcc;padding-left:1.5em'>" + text2 + "</ul>");
            }
        }
    }
}
