// Copyright 2011 Miyako Komooka
using System;
using System.IO;
// using System.Collections;
using System.Collections.Generic;
using System.Globalization;

// Property Value Datat Types, Defined in rfc5545 section 3.3
namespace iCalLibrary.DataType
{
    using Parameter;

    public abstract class iCalDataType /* : ICloneable */ {
        public virtual Object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public abstract class iCalTimeRelatedType : iCalDataType, IComparable<iCalTimeRelatedType>
    {
        public override int GetHashCode(){
            return base.GetHashCode();
        }

        public virtual iCalTimeRelatedType Plus( iCalDurationDataType duration )
        {
            return null;
        }

        public virtual iCalDurationDataType Minus( iCalTimeRelatedType date2 )
        {
            return null;
        }

        public static iCalTimeRelatedType operator +
            ( iCalTimeRelatedType time, iCalDurationDataType duration ){
            return time.Plus( duration );
        }

        public static iCalDurationDataType operator -
            ( iCalTimeRelatedType date1, iCalTimeRelatedType date2 ){
            return date1.Minus( date2 );
        }

        public virtual int CompareTo( iCalTimeRelatedType date2 )
        {
             return 0;
        }

        public override bool Equals( Object date2 )
        {
            return false;
        }

        public virtual DayOfWeek GetDayOfWeek()
        {
            return DayOfWeek.Monday;
        }

        public virtual int Year  { get{ return 0; } set{} }
        public virtual int Month { get{ return 0; } set{} }
        public virtual int Day   { get{ return 0; } set{} }

        public static bool operator == ( iCalTimeRelatedType date1,
                                         iCalTimeRelatedType date2) {

            if( ((Object)date1) == null ){
                if( ((Object)date2) == null ){
                    return true;
                }
            } else  if( date1.Equals( date2 ) ){
                return true;
            }
            return false;
        }

        public static bool operator != ( iCalTimeRelatedType date1,
                                         iCalTimeRelatedType date2) {
            if( date1 == date2 ){
                return false;
            }
            return true;
        }

        public static bool operator < (iCalTimeRelatedType date1,
                                       iCalTimeRelatedType date2)
        {
            int ret = date1.CompareTo( date2 );
            if( ret < 0 ){
                return true;
            }
            return false;
        }

        public static bool operator >(iCalTimeRelatedType date1,
                                      iCalTimeRelatedType date2)
        {
            int ret = date1.CompareTo( date2 );

            if( ret > 0 ){
                return true;
            }
            return false;
        }

        public static bool operator <= (iCalTimeRelatedType date1,
                                        iCalTimeRelatedType date2)
        {
            int ret = date1.CompareTo( date2 );
            if( ret <= 0 ){
                return true;
            }
            return false;
        }

        public static bool operator >= (iCalTimeRelatedType date1,
                                        iCalTimeRelatedType date2)
        {
            int ret = date1.CompareTo( date2 );
            if( ret >= 0 ){
                return true;
            }
            return false;
        }

        // public static bool operator < ( iCalDate date2, iCalTimeRelatedType date1 ) {
        //     if( date1 > date2 ){
        //         return true;
        //     }
        //     return false;
        // }

        // public static bool operator > ( iCalDate date2, iCalTimeRelatedType date1 ) {
        //     if( date1 < date2 ){
        //         return true;
        //     }
        //     return false;
        // }

    }

    // date-time = date "T" time
    public class iCalDateTime  : iCalTimeRelatedType
    {
        public iCalDate Date;
        public iCalTime Time;

        public iCalDateTime(){
            this.Date = new iCalDate();
            this.Time = new iCalTime();
        }

        public iCalDateTime(String str)
        {
            str = str.ToLower();
            int index = str.IndexOf( "t" );
            String date, time;
            if( index < 0 ){
                date = str;
                this.Time = null;
            } else {
                date = str.Substring( 0, index );
                time = str.Substring( index + 1 );
                this.Time = new iCalTime( time );
            }
            this.Date = new iCalDate( date );
        }

        public iCalDateTime( iCalDate date )
        {
            this.Date = date;
            this.Time = null;
        }

        public iCalDateTime( System.DateTime systemDateTime ){
            iCalDate retDate = new iCalDate();
            retDate.Year  = systemDateTime.Year;
            retDate.Month = systemDateTime.Month;
            retDate.Day   = systemDateTime.Day;

            iCalTime retTime = new iCalTime();
            retTime.Hour   = systemDateTime.Hour;
            retTime.Minute = systemDateTime.Minute;
            retTime.Second = systemDateTime.Second;
            if( systemDateTime.Kind == DateTimeKind.Utc ){
                retTime.IsUTC  = true;
            } else {
                retTime.IsUTC  = false;
            }

            this.Date = retDate;
            this.Time = retTime;
        }

        public override int Year {
            get{
                if( this.Date != null ) {
                    return this.Date.Year;
                }
                return 0;
            }
                
            set{
                if( this.Date != null ) {
                    this.Date.Year = value;
                }
            }
        }

        public override int Month {
            get{
                if( this.Date != null ) {
                    return this.Date.Month;
                }
                return 0;
            }
                
            set{
                if( this.Date != null ) {
                    this.Date.Month = value;
                }
            }
        }

        public override int Day {
            get{
                if( this.Date != null ) {
                    return this.Date.Day;
                }
                return 0;
            }
                
            set{
                if( this.Date != null ) {
                    this.Date.Month = Day;
                }
            }
        }

        public override Object Clone()
        {
            iCalDateTime ret = (iCalDateTime)base.Clone();
            if( this.Date != null ){
                // ret.Date = new iCalDate();
                // ret.Date.Year = this.Date.Year;
                // ret.Date.Month = this.Date.Month;
                // ret.Date.Day = this.Date.Day;
                if( this.Date != null ){
                    ret.Date = (iCalDate)this.Date.Clone();
                }
            }

            if( this.Time != null ){
                // ret.Time = new iCalTime();
                // ret.Time.IsUTC = this.Time.IsUTC;
                // ret.Time.Hour = this.Time.Hour;
                // ret.Time.Minute = this.Time.Minute;
                // ret.Time.Second = this.Time.Second;
                if( this.Time != null ){
                    ret.Time = (iCalTime)this.Time.Clone();
                }
            }

            return ret;
        }

        public override String ToString()
        {
            String ret = "";

            if( this.Date != null ){
                ret += this.Date.ToString();
            }
            if( this.Time != null ){
                ret += "T" + this.Time.ToString();
            }
            return ret;
        }

        public override int GetHashCode(){
            return this.Date.GetHashCode() + this.Time.GetHashCode();
        }

        public override int CompareTo( iCalTimeRelatedType date2 )
        {
            iCalDateTime datetime = date2 as iCalDateTime;

            if( datetime != null ){
                int ret = this.Date.CompareTo( datetime.Date );
                if( ret == 0 ){
                    ret = this.Time.CompareTo( datetime.Time );
                }
                return ret;
            } else {
                throw new ArgumentException( "cannot compare iCalDateTime with over object" );
            }
        }

        public override bool Equals( Object obj )
        {
            iCalDateTime datetime = obj as iCalDateTime;
            iCalDate date = obj as iCalDate;

            if( datetime != null ){
                if( this.Time == datetime.Time &&
                    this.Date == datetime.Date ){
                    return true;
                }
            } else if( date != null ){
                if( this.Date == date ){
                    return true;
                }
            }
            return false;
        }

        public override iCalTimeRelatedType Plus( iCalDurationDataType duration )

        {
            DateTime systemDateTime = (DateTime)this;

            if( systemDateTime != null ){
                int week = duration.Week;
                int day = duration.Day;
                int hour = duration.Hour;
                int minute = duration.Minute;
                int second = duration.Second;
                
                if( duration.IsMinus ){
                    week = -week;
                    day = -day;
                    hour = -hour;
                    minute = -minute;
                    second = -second;
                }

                systemDateTime = systemDateTime.AddDays(week * 7);
                systemDateTime = systemDateTime.AddDays(day);
                systemDateTime = systemDateTime.AddHours(hour);
                systemDateTime = systemDateTime.AddMinutes(minute);
                systemDateTime = systemDateTime.AddSeconds(second);
                
                return new iCalDateTime( systemDateTime );
            }
            return null;
        }

        public override iCalDurationDataType Minus( iCalTimeRelatedType date2 )
        {
            iCalDurationDataType ret = new iCalDurationDataType();
            
            iCalDateTime datetime = date2 as iCalDateTime;
            iCalDate     date     = date2 as iCalDate;
            

            if( datetime != null ){
                DateTime systemDateTime = (DateTime)this;
                TimeSpan span = systemDateTime - (DateTime)datetime;
                ret.Day = span.Days;
                ret.Hour = span.Hours;
                ret.Minute = span.Minutes;
                ret.Second = span.Seconds;
            } else if( date != null ){
                DateTime systemDateTime = (DateTime)this;
                TimeSpan span = systemDateTime - (DateTime)date;
                ret.Day = span.Days;
                ret.Hour = span.Hours;
                ret.Minute = span.Minutes;
                ret.Second = span.Seconds;
            }
            if( ret.Day < 0 ){
                ret.IsMinus = true;
                ret.Day = -ret.Day;
            }
            if( ret.Hour < 0 ){
                ret.IsMinus = true;
                ret.Hour = -ret.Hour;
            }
            if( ret.Minute < 0 ){
                ret.IsMinus = true;
                ret.Minute = -ret.Minute;
            }
            if( ret.Second < 0 ){
                ret.IsMinus = true;
                ret.Second = -ret.Second;
            }

            return ret;
        }

        public override DayOfWeek GetDayOfWeek()
        {
            DateTime systemDateTime = (DateTime)this;
            return systemDateTime.DayOfWeek;
        }


        public static iCalDateTime operator- ( iCalDateTime datetime,
                                               iCalUTCOffset offset )
        {
            DateTime systemDateTime = (DateTime)datetime;

            int offsetHour = offset.Hour;
            int offsetMinute = offset.Minute;
            int offsetSecond = offset.Second;
            if( offset.IsMinus ){
                offsetHour   = -offsetHour;
                offsetMinute = -offsetMinute;
                offsetSecond = -offsetSecond;
            }

            systemDateTime = systemDateTime.AddHours( -offsetHour );
            systemDateTime = systemDateTime.AddMinutes( -offsetMinute );
            systemDateTime = systemDateTime.AddSeconds( -offsetSecond );

            iCalDateTime ret = new iCalDateTime( systemDateTime );
            if (systemDateTime.Kind == DateTimeKind.Utc) {
                ret.Time.IsUTC = true;
            } else {
                ret.Time.IsUTC = false;
            }

            return ret;
        }

        // public static bool operator > ( iCalDateTime datetime1, iCalDate date2 )
        // {
        //     if( datetime1.LessThan( date2 ) ){
        //         return true;
        //     }
        //     return false;
        // }

        // public static bool operator < (iCalDateTime datetime1, iCalDate date2) {
        //     if ( datetime1 > date2 ) {
        //         return false;
        //     }
        //     return true; /**/
        // }

        public static explicit operator System.DateTime(iCalDateTime datetime)
        {
            System.DateTime ret;

            int year=0, month=0, day=0;
            int hour=0, minute=0, second=0;
            bool isutc = false;

            if( datetime.Date != null ){
                year  = datetime.Date.Year;
                month = datetime.Date.Month;
                day   = datetime.Date.Day;
            }
            if( datetime.Time != null ){
                hour = datetime.Time.Hour;
                minute = datetime.Time.Minute;
                second = datetime.Time.Second;
                isutc  = datetime.Time.IsUTC;
            }
            if( isutc ){
                ret = new DateTime( year, month, day,
                                    hour, minute, second,
                                    DateTimeKind.Utc );
            } else {
                ret = new DateTime( year, month, day,
                                    hour, minute, second );
            }

            return ret;
        }

        public static explicit operator iCalDate ( iCalDateTime datetime )
        {
            if( datetime.Time == null ){
                return datetime.Date;
            }
            return null;
        }

        public static explicit operator iCalTime ( iCalDateTime datetime )
        {
            if( datetime.Date == null ){
                return datetime.Time;
            }
            return null;
        }
    }

    //  date = YYYYMMDD
    public class iCalDate : iCalTimeRelatedType
    {
        public int year = 1;
        public override int Year {
            get{ return year; }
            set{ year = value; }
        }
        
        public int month = 1;
        public override int Month {
            get{ return month; }
            set{ month = value; }
        }
        
        public int day = 1;
        public override int Day {
            get{ return day; }
            set{ day = value; }
        }
            
        public iCalDate(){}

        public iCalDate( String str ){
            if( str.Length != 8 ){
                return;
            }
            this.Year = Int32.Parse( str.Substring( 0, 4 ) );
            str = str.Substring( 4 );
            this.Month = Int32.Parse( str.Substring( 0, 2 ) );
            str = str.Substring( 2 );
            this.Day = Int32.Parse( str.Substring( 0, 2 ) );

        }

        public iCalDate( DateTime systemDateTime ){
            this.Year = systemDateTime.Year;
            this.Month = systemDateTime.Month;
            this.Day   = systemDateTime.Day;
        }

        public override Object Clone()
        {
            iCalDate ret = (iCalDate)base.Clone();
            ret.Year = this.Year;
            ret.Month = this.Month;
            ret.Day = this.Day;

            return ret;
        }

        public override String ToString()
        {
            return( this.Year.ToString( "0000" ) + "/" +
                    this.Month.ToString( "00" ) + "/" +
                    this.Day.ToString( "00" ) );
        }

        public override int GetHashCode(){
            return this.Year + this.Month + this.Day;
        }

        public override int CompareTo( iCalTimeRelatedType date2 )
        {
            if (date2 == null) {
                throw new ArgumentNullException();
            }

            iCalDateTime datetime = date2 as iCalDateTime;
            iCalDate date = date2 as iCalDate;

            if( datetime != null ){
                date = datetime.Date;
            }


            if( date != null ){
                int ret = this.Year - date.Year;
                
                if( ret == 0 ){
                    ret = this.Month - date.Month;
                }

                if( ret == 0 ){
                    ret = this.Day - date.Day;
                }

                return ret;
            }

            throw new ArgumentException( "cannot compare iCalDate with unkown type" );
        }

        public override bool Equals( Object obj ){
            if (obj == null) {
                return false;
            }

            iCalDate date2 = obj as iCalDate;
            iCalDateTime datetime = obj as iCalDateTime;

            if( date2 != null ){
                if(  this.Year == date2.Year &&
                     this.Month == date2.Month &&
                     this.Day == date2.Day ){
                    return true;
                }
            } else if( datetime != null ){
                if( datetime.Date == this ){
                    return true;
                }
            }
            return false;
        }

        public override iCalTimeRelatedType Plus( iCalDurationDataType duration )
        {
            iCalDateTime datetime = new iCalDateTime( this );
            iCalDateTime ret = (iCalDateTime)(datetime + duration);
            if( ret.Time == null || ret.Time.IsZero() ){
                return ret.Date;
            }
            return ret;
        }

        public override iCalDurationDataType Minus( iCalTimeRelatedType date2 )
        {
            iCalDateTime datetime = new iCalDateTime( this );
            iCalDurationDataType ret = datetime - date2;
            return ret;
        }

        public iCalDate NextDay()
        {
            DateTime systemDateTime = (DateTime)this;
            systemDateTime = systemDateTime.AddDays( 1 );

            return new iCalDate( systemDateTime );
        }

        public iCalDate PreviousDay()
        {
            DateTime systemDateTime = (DateTime)this;
            systemDateTime = systemDateTime.AddDays( -1 );

            return new iCalDate( systemDateTime );
        }

        public override DayOfWeek GetDayOfWeek()
        {
            DateTime systemDateTime = (DateTime)this;
            return systemDateTime.DayOfWeek;
        }

        public static explicit operator System.DateTime(iCalDate date)
        {
            System.DateTime ret = 
                new System.DateTime( date.Year,
                                     date.Month,
                                     date.Day );

            return ret;
        }


        public static bool operator < ( iCalDate date1, iCalDate date2 ){
            int ret = date1.CompareTo( date2 );
            
            if( ret < 0 ){
                return true;
            }
            return false;
        }

        public static bool operator >(iCalDate date1, iCalDate date2) {
            if( date1 <= date2 ){
                return false;
            }
            return true;
        }

        public static bool operator == ( iCalDate date1, iCalDate date2 ){

            if( ((Object)date1) == null ){
                if( ((Object)date2) == null ){
                    return true;
                }
            } else if( date1.Equals( date2 ) ){
                return true;
            }
            return false;
        }

        public static bool operator != ( iCalDate date1, iCalDate date2 ){
            if( date1 == date2 ){
                return false;
            }
            return true;
        }

        public static bool operator <= ( iCalDate date1, iCalDate date2 ){
            if( date1 < date2 || date1 == date2 ){
                return true;
            }
            return false;
        }

        public static bool operator >= (iCalDate date1, iCalDate date2){
            if (date1 > date2 || date1 == date2) {
                return true;
            }
            return false;
        }
    }

    // time = HHMMSS[z]
    public class iCalTime : iCalTimeRelatedType
    {
        public bool IsUTC = false;
        public int Hour = 0;
        public int Minute = 0;
        public int Second = 0;

        public iCalTime(){}

        public iCalTime( String str ){
            
            if( str.Length < 6 ){
                return ;
            }

            this.Hour = Int32.Parse( str.Substring( 0, 2 ) );
            str = str.Substring( 2 );
            this.Minute = Int32.Parse( str.Substring( 0, 2 ) );
            str = str.Substring( 2 );
            this.Second = Int32.Parse( str.Substring( 0, 2 ) );
            str = str.Substring( 2 );

            if( str.Length > 0 && str.ToLower()[0] == 'z' ){
                this.IsUTC = true;
            } else {
                this.IsUTC = false;
            }
        }

        public override Object Clone()
        {
            iCalTime ret = (iCalTime)base.Clone();
            ret.IsUTC = this.IsUTC;
            ret.Hour = this.Hour;
            ret.Minute = this.Minute;
            ret.Second = this.Second;

            return ret;
        }

        public override String ToString()
        {
            String ret = this.Hour.ToString( "00" ) + ":" +
                this.Minute.ToString( "00" ) + ":" +
                this.Second.ToString( "00" );

            if( this.IsUTC ){
                ret += "z";
            }

            return ret;
        }

        public override int GetHashCode(){
            return this.Hour + this.Minute + this.Second;
        }

        public override bool Equals( Object date2 )
        {
            iCalTime time = date2 as iCalTime;
            if( time != null &&
                this.Hour == time.Hour &&
                this.Minute == time.Minute &&
                this.Second == time.Second &&
                this.IsUTC == time.IsUTC ){
                return true;
            }
            return false;
        }

        public override int CompareTo( iCalTimeRelatedType date2 )
        {
            iCalTime time = date2 as iCalTime;

            if( time != null ){
                if( this.IsUTC != time.IsUTC ){

                    throw new ArgumentException( "cannot compare non utc time and utc time" );
                }

                int ret = this.Hour - time.Hour;

                if( ret == 0 ){
                    ret = this.Minute - time.Minute;
                }

                if( ret == 0 ){
                    ret = this.Second - time.Second;
                }
                
                return ret;
            }
            throw new ArgumentException( "cannot compare iCalTime and other" );
        }

        public bool IsZero(){
            if( this.Hour == 0 &&
                this.Minute == 0 &&
                this.Second == 0 ){
                return true;
            }
            return false;
        }

        public static bool operator == ( iCalTime date1, iCalTime date2) {
            if( ((Object)date1) == null ){
                if( ((Object)date2) == null ){
                    return true;
                }
            } else  if( date1.Equals( date2 ) ){
                return true;
            }
            return false;
        }

        public static bool operator != ( iCalTime date1, iCalTime date2) {
            if( date1 == date2 ){
                return false;
            }
            return true;
        }
    }

    public class iCalDurationDataType  : iCalDataType
    {
        // dur-value  = (["+"] / "-") "P" (dur-date / dur-time / dur-week)
        // dur-date   = dur-day [dur-time]
        // dur-time   = "T" (dur-hour / dur-minute / dur-second)
        // dur-week   = 1*DIGIT "W"
        // dur-hour   = 1*DIGIT "H" [dur-minute]
        // dur-minute = 1*DIGIT "M" [dur-second]
        // dur-second = 1*DIGIT "S"
        // dur-day    = 1*DIGIT "D"

        public bool IsMinus = false;
        public int Week = 0;
        public int Day = 0;
        public int Hour = 0;
        public int Minute = 0;
        public int Second = 0;

        public iCalDurationDataType(){}
        
        public iCalDurationDataType( String str ){
            str = str.ToLower();

            if( str.Length > 0 ){
                if( str[0] == '-' ){
                    this.IsMinus = true;
                    str = str.Substring( 1 );
                } else if( str[0] == '+' ){
                    this.IsMinus = false;
                    str = str.Substring( 1 );
                }
            }
            if( str.Length > 0 ){
                if( str[0] == 'p' ){
                    str = str.Substring( 1 );
                } else {
                    return;
                }
            }
            
            String str1 = "";
            for( int i = 0; i < str.Length; i++ ){
                Char c = str[i];
                if ( Char.IsDigit( c ) ){
                    str1 += c;
                } else if( c == 't' ){
                    str1 = "";
                } else if( c == 'd' ){
                    this.Day = Int32.Parse( str1 );
                    str1 = "";
                } else if( c == 'w' ){
                    this.Week = Int32.Parse( str1 );
                    str1 = "";
                } else if( c == 'h' ){
                    this.Hour = Int32.Parse( str1 );
                    str1 = "";
                } else if( c == 'm' ){
                    this.Minute = Int32.Parse( str1 );
                    str1 = "";
                } else if( c == 's' ){
                    this.Second = Int32.Parse( str1 );
                    str1 = "";
                }
            }
        }

        public override Object Clone()
        {
            iCalDurationDataType ret = (iCalDurationDataType)base.Clone();

            ret.IsMinus = this.IsMinus;
            ret.Week = this.Week;
            ret.Day = this.Day;
            ret.Hour = this.Hour;
            ret.Minute = this.Minute;
            ret.Second = this.Second;

            return ret;
        }

        public bool IsZero()
        {
            if( this.Week == 0 && this.Day == 0 &&
                this.Hour == 0 && this.Minute == 0 && this.Second == 0 ){
                return true;
            }
            return false;
        }

        // public static iCalDateTime operator + ( iCalDateTime datetime,
        //                                         iCalDurationDataType duration )
        // {
        //     DateTime systemDateTime = (DateTime)datetime;

        //     int week = duration.Week;
        //     int day = duration.Day;
        //     int hour = duration.Hour;
        //     int minute = duration.Minute;
        //     int second = duration.Second;
            
        //     if( duration.IsMinus ){
        //         week = -week;
        //         day = -day;
        //         hour = -hour;
        //         minute = -minute;
        //         second = -second;
        //     }

        //     systemDateTime.AddDays( week * 7 );
        //     systemDateTime.AddDays( day );
        //     systemDateTime.AddHours( hour );
        //     systemDateTime.AddMinutes( minute );
        //     systemDateTime.AddSeconds( second );

        //     return new iCalDateTime( systemDateTime );
        // }

        // public static iCalDateTime operator + ( iCalDate date,
        //                                         iCalDurationDataType duration )
        // {
        //     iCalDateTime datetime = new iCalDateTime( date );
        //     iCalDateTime ret = datetime + duration;
        //     return ret;
        // }

    }

    public class iCalPeriod  : iCalTimeRelatedType
    {
        // period     = period-explicit / period-start
        // period-explicit = date-time "/" date-time
        // period-start = date-time "/" dur-value

        public bool UseDuration = false; // false if start/end time specified
                                         // true  if start time/duration specified
        public iCalDateTime Start;
        public iCalDateTime End;
        public iCalDurationDataType Duration;

        public iCalPeriod(){}

        public iCalPeriod( String str ){
            str = str.ToLower();

            String str1, str2;
            int index = str.IndexOf( '/' );
            if( index < 0 ){
                str1 = str;
                str2 = "p0s";
            } else {
                str1 = str.Substring( 0, index );
                str2 = str.Substring( index + 1 );
            }

            this.Start = new iCalDateTime( str1 );
            if( str2.IndexOf( 'p' ) < 0 ){
                this.End = new iCalDateTime( str2 );
                this.UseDuration = false;
            } else {
                this.Duration = new iCalDurationDataType( str2 );
                this.UseDuration = true;
            }
        }

        public override Object Clone()
        {
            iCalPeriod ret = (iCalPeriod)base.Clone();

            ret.UseDuration = this.UseDuration;
            if( this.Start != null ){
                ret.Start = (iCalDateTime)this.Start.Clone();
            }

            if( this.End != null ){
                ret.End = (iCalDateTime)this.End.Clone();
            }

            if( this.Duration != null ){
                ret.Duration = (iCalDurationDataType)this.Duration.Clone();
            }

            return ret;
        }

    }

    public class iCalDayOfWeek : iCalDataType, IComparable<iCalDayOfWeek>
    {
        // weekdaynum  = [[plus / minus] ordwk] weekday
        // plus        = "+"
        // minus       = "-"
        // ordwk       = 1*2DIGIT       ;1 to 53
        // weekday     = "SU" / "MO" / "TU" / "WE" / "TH" / "FR" / "SA"

        public bool IsMinus = false;
        public int  NumOfWeek;
        public DayOfWeek DayOfWeek;

        public iCalDayOfWeek(){}

        public iCalDayOfWeek( String str ){
            if( str == null || str.Length == 0 )
                return;

            if( str[0] == '-' ){
                this.IsMinus = true;
                str = str.Substring( 1 );
            } else if( str[0] == '+' ){
                this.IsMinus = false;
                str = str.Substring( 1 );
            }

            String str1 = "";
            for( int i = 0; i < str.Length; i++ ){
                Char c = str[i];
                if ( Char.IsDigit( c ) ){
                    str1 += c;
                } else {
                    break;
                }
            }
            if( str1.Length == 0 ){
                this.NumOfWeek = 0;
            } else {
                this.NumOfWeek = Int32.Parse( str1 );
            }

            str = str.Substring(str1.Length);
            str = str.ToLower();
            if( str == "su" ){
                this.DayOfWeek = DayOfWeek.Sunday;
            } else if( str == "mo" ){
                this.DayOfWeek = DayOfWeek.Monday;
            } else if( str == "tu" ){
                this.DayOfWeek = DayOfWeek.Tuesday;
            } else if( str == "we" ){
                this.DayOfWeek = DayOfWeek.Wednesday;
            } else if( str == "th" ){
                this.DayOfWeek = DayOfWeek.Thursday;
            } else if( str == "fr" ){
                this.DayOfWeek = DayOfWeek.Friday;
            } else if( str == "sa" ){
                this.DayOfWeek = DayOfWeek.Saturday;
            }
        }

        public override Object Clone()
        {
            iCalDayOfWeek ret = (iCalDayOfWeek)base.Clone();

            ret.IsMinus = this.IsMinus;
            ret.NumOfWeek = this.NumOfWeek;
            ret.DayOfWeek = this.DayOfWeek;

            return ret;
        }

        public int CompareTo( iCalDayOfWeek other ){
            int thisnum = this.NumOfWeek;
            if( this.IsMinus ){
                thisnum = -thisnum;
            }
            int othernum = other.NumOfWeek;
            if( other.IsMinus ){
                othernum = -othernum;
            }

            int ret = thisnum - othernum;
            if( ret == 0 ){
                ret = (int)(this.DayOfWeek) - (int)other.DayOfWeek;
            }
            return ret;
        }

        // returns date if NumOfWeek specified (1MO ... )
        // returns first date is NumOfWeek not specified
        public iCalDate GetFirstDate( int year, int month ){
            int num = this.NumOfWeek;
            if( num == 0 ){
                num = 1;
            }

            Calendar systemCalendar = new GregorianCalendar();
            int lastDay = systemCalendar.GetDaysInMonth( year, month );
            DateTime systemDateTime;

            if( this.IsMinus ){
                systemDateTime = new DateTime( year, month, lastDay );
            } else {
                systemDateTime = new DateTime( year, month, 1 );
            }

            while( true ){
                if( systemDateTime.DayOfWeek == this.DayOfWeek ){
                    break;
                }
                if( this.IsMinus ){
                    systemDateTime = systemDateTime.AddDays( -1 );
                } else {
                    systemDateTime = systemDateTime.AddDays( 1 );
                }
            }

            if( num == 1 ){
                return new iCalDate( systemDateTime );
            }

            DateTime systemDateTime2;
            if( this.IsMinus ){
                systemDateTime2 = systemDateTime.AddDays( (num-1) * -7 );
            } else {
                systemDateTime2 = systemDateTime.AddDays( (num-1) * 7 );
            }

            if( systemDateTime.Year == systemDateTime2.Year &&
                systemDateTime.Month == systemDateTime2.Month ){
                return new iCalDate( systemDateTime2 );
            } else {
                return null;
            }
        }

        public iCalDate GetFirstDate( int year ){
            int num = this.NumOfWeek;
            if( num == 0 ){
                num = 1;
            }

            Calendar systemCalendar = new GregorianCalendar();
            int lastDay = systemCalendar.GetDaysInMonth( year, 12 );

            DateTime systemDateTime;

            if( this.IsMinus ){
                systemDateTime = new DateTime( year, 12, lastDay );
            } else {
                systemDateTime = new DateTime( year, 1, 1 );
            }

            while( true ){
                if( systemDateTime.DayOfWeek == this.DayOfWeek ){
                    break;
                }
                if( this.IsMinus ){
                    systemDateTime = systemDateTime.AddDays( -1 );
                } else {
                    systemDateTime = systemDateTime.AddDays( 1 );
                }
            }

            if( num == 1 ){
                return new iCalDate( systemDateTime );
            }

            DateTime systemDateTime2;
            if( this.IsMinus ){
                systemDateTime2 = systemDateTime.AddDays( (num-1) * -7 );
            } else {
                systemDateTime2 = systemDateTime.AddDays( (num-1) * 7 );
            }
            
            if( systemDateTime.Year == systemDateTime2.Year ){
                return new iCalDate( systemDateTime2 );
            } else {
                return null;
            }
        }
    }

    public class iCalText : iCalDataType
    {
        // text       = *(TSAFE-CHAR / ":" / DQUOTE / ESCAPED-CHAR)
        //    ; Folded according to description above
        // ESCAPED-CHAR = ("\\" / "\;" / "\," / "\N" / "\n")
        //    ; \\ encodes \, \N or \n encodes newline
        //    ; \; encodes ;, \, encodes ,
        // TSAFE-CHAR = WSP / %x21 / %x23-2B / %x2D-39 / %x3C-5B /
        //              %x5D-7E / NON-US-ASCII
        //    ; Any character except CONTROLs not needed by the current
        //    ; character set, DQUOTE, ";", ":", "\", ","
        public String Text;

        public iCalText(){}
        
        public iCalText( String str )
        {
            this.Text = "";
            if( str == null ){
                return;
            }

            for( int i = 0; i < str.Length; i++ ){
                if( str[i] == '\\' ){
                    i++;
                    if( str[i] == '\\' ){
                        this.Text += '\\';
                    } else if( str[i] == ';' ){
                        this.Text += ';';
                    } else if( str[i] == ',' ){
                        this.Text += ',';
                    } else if( str[i] == 'N' ){
                        this.Text += '\n';
                    } else if( str[i] == 'n' ){
                        this.Text += '\n';
                    }
                } else {
                    this.Text += str[i];
                }
            }
        }

        public override Object Clone()
        {
            iCalText ret = (iCalText)base.Clone();

            ret.Text = this.Text;

            return ret;
        }
        
        public override String ToString()
        {
            return this.Text;
        }

        public static List<iCalText> ParseMultipleTexts( String str )
        {
            String str1 = "";
            List<iCalText> ret = new List<iCalText>();

            for( int i=0; i < str.Length; i++ ){
                if( str[i] == '\\' ){
                    str1 += str[i];
                    i++;
                    str1 += str[i];
                } else if( str[i] == ',' ) {
                    ret.Add( new iCalText( str1 ) );
                    str1 = "";
                } else {
                    str1 += str[i];
                }
            }
            ret.Add( new iCalText( str1 ) );

            return ret;
        }

        public override bool Equals( Object obj ){
            iCalText text = obj as iCalText;

            if( text != null &&  this.Text == text.Text ){
                return true;
            }
            return false;
        }

        public override int GetHashCode(){
            return this.Text.GetHashCode();
        }

        public static bool operator == ( iCalText text1, iCalText text2 ) {

            if( ((Object)text1) == null ){
                if( ((Object)text2) == null ){
                    return true;
                }
            } else  if( text1.Equals( text2 ) ){
                return true;
            }
            return false;
        }

        public static bool operator != ( iCalText text1, iCalText text2 ) {
            if( text1 == text2 ){
                return false;
            }
            return true;
        }
        
    }

    public class iCalUTCOffset : iCalDataType
    {
        // utc-offset = time-numzone
        // time-numzone = ("+" / "-") time-hour time-minute [time-second]
        // time-hour    = 2DIGIT        ;00-23
        // time-minute  = 2DIGIT        ;00-59
        // time-second  = 2DIGIT        ;00-60
        // ;The "60" value is used to account for positive "leap" seconds.

        public bool IsMinus = false;
        public int Hour = 0;
        public int Minute = 0;
        public int Second = 0;

        public iCalUTCOffset(){}

        public iCalUTCOffset( String str ){
            if( str.Length < 4 ){
                return;
            }

            if( str[0] == '-' ){
                this.IsMinus = true;
                str = str.Substring( 1 );
            } else if( str[0] == '+' ){
                this.IsMinus = false;
                str = str.Substring( 1 );
            }

            this.Hour = Int32.Parse( str.Substring( 0, 2 ) );
            str = str.Substring( 2 );
            this.Minute = Int32.Parse( str.Substring( 0, 2 ) );
            str = str.Substring( 2 );
            if( str.Length > 0 ){
                this.Second = Int32.Parse( str );
            }

            return;
        }

        public override Object Clone()
        {
            iCalUTCOffset ret = (iCalUTCOffset)base.Clone();

            ret.IsMinus = this.IsMinus;
            ret.Hour = this.Hour;
            ret.Minute = this.Minute;
            ret.Second = this.Second;

            return ret;
        }

        public override String ToString(){
            String ret = "";
            if( this.IsMinus ){
                ret += "-";
            } else {
                ret += "+";
            }
            ret += this.Hour.ToString( "00" )  + ":" +
                   this.Minute.ToString( "00" ) + ":" +
                   this.Second.ToString( "00" );

            return ret;
        }
    }
}