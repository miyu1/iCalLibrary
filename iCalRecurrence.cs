// Copyright 2011 Miyako Komooka
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace iCalLibrary.DataType
{
    using Parameter;

    public class SortedSet<T> /* : ISet<T> */
    {
        List<T> list = new List<T>();

        public int Count { get { return list.Count; } }

        public bool Add( T item ){
            if( list.Contains( item ) ){
                return false;
            }
            list.Add( item );
            list.Sort();
            return true;
        }

        public bool Contains( T item ){
            if( list.Contains( item ) ){
                return true;
            }
            return false;
        }

        /* IEnumerator IEnumerable.GetEnumerator(){
            return list.GetEnumerator();
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator(){
            return list.GetEnumerator();
            } */

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }

    public class iCalRecurrence : iCalDataType
    {
        // recur           = recur-rule-part *( ";" recur-rule-part )
        // recur-rule-part = ( "FREQ" "=" freq )
        //                 / ( "UNTIL" "=" enddate )
        //                 / ( "COUNT" "=" 1*DIGIT )
        //                 / ( "INTERVAL" "=" 1*DIGIT )
        //                 / ( "BYSECOND" "=" byseclist )
        //                 / ( "BYMINUTE" "=" byminlist )
        //                 / ( "BYHOUR" "=" byhrlist )
        //                 / ( "BYDAY" "=" bywdaylist )
        //                 / ( "BYMONTHDAY" "=" bymodaylist )
        //                 / ( "BYYEARDAY" "=" byyrdaylist )
        //                 / ( "BYWEEKNO" "=" bywknolist )
        //                 / ( "BYMONTH" "=" bymolist )
        //                 / ( "BYSETPOS" "=" bysplist )
        //                 / ( "WKST" "=" weekday )
        // freq        = "SECONDLY" / "MINUTELY" / "HOURLY" / "DAILY"
        //             / "WEEKLY" / "MONTHLY" / "YEARLY"
        // enddate     = date / date-time
        // byseclist   = ( seconds *("," seconds) )
        // seconds     = 1*2DIGIT       ;0 to 60
        // byminlist   = ( minutes *("," minutes) )
        // minutes     = 1*2DIGIT       ;0 to 59
        // byhrlist    = ( hour *("," hour) )
        // hour        = 1*2DIGIT       ;0 to 23
        // bywdaylist  = ( weekdaynum *("," weekdaynum) )
        // weekdaynum  = [[plus / minus] ordwk] weekday
        // plus        = "+"
        // minus       = "-"
        // ordwk       = 1*2DIGIT       ;1 to 53
        // weekday     = "SU" / "MO" / "TU" / "WE" / "TH" / "FR" / "SA"
        // ;Corresponding to SUNDAY, MONDAY, TUESDAY, WEDNESDAY, THURSDAY,
        // ;FRIDAY, and SATURDAY days of the week.
        // bymodaylist = ( monthdaynum *("," monthdaynum) )
        // monthdaynum = [plus / minus] ordmoday
        // ordmoday    = 1*2DIGIT       ;1 to 31
        // byyrdaylist = ( yeardaynum *("," yeardaynum) )
        // yeardaynum  = [plus / minus] ordyrday
        // ordyrday    = 1*3DIGIT      ;1 to 366
        // bywknolist  = ( weeknum *("," weeknum) )
        // weeknum     = [plus / minus] ordwk
        // bymolist    = ( monthnum *("," monthnum) )
        // monthnum    = 1*2DIGIT       ;1 to 12
        // bysplist    = ( setposday *("," setposday) )
        // setposday   = yeardaynum

        public enum Frequency { Secondly, Minutely, Hourly, Daily, Weekly, Monthly, Yearly }

        public Frequency Freq = 0;
        public iCalTimeRelatedType Until = null;
        public int Count = 0;
        public int Interval = 1;
        public SortedSet<int> BySecList = new SortedSet<int>();
        public SortedSet<int> ByMinList = new SortedSet<int>();
        public SortedSet<int> ByHourList = new SortedSet<int>();
        public SortedSet<iCalDayOfWeek> ByWeekDayList =
            new SortedSet<iCalDayOfWeek>();

        // -31..-1, 1..31
        public SortedSet<int> ByMonthDayList = new SortedSet<int>();
        // -366..-1, 1..366
        public SortedSet<int> ByYearDayList = new SortedSet<int>();
        // -53..-1, 1..53        
        public SortedSet<int> ByWeeknoList = new SortedSet<int>();
        // -12..-1, 1..12
        public SortedSet<int> ByMonthList = new SortedSet<int>();
        // -366..-1, 1..366
        public SortedSet<int> BySetposList = new SortedSet<int>();
        
        public DayOfWeek WeekStart = DayOfWeek.Monday;

        public iCalRecurrence(){}

        public iCalRecurrence( String str )
        {
            str = "DUMMYNAME;" + str + ":DUMMYVALUE\n";
            StringReader sReader = new StringReader( str );
            iCalReader reader = new iCalReader( sReader );
            iCalLineContent content = reader.ReadContent();

            
            iCalParameter param;
            String key;

            // freq
            key = "freq";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                String value = param.Value().ToLower(); 
                if( value == "secondly" ){
                    this.Freq = Frequency.Secondly;
                } else if( value == "minutely" ){
                    this.Freq = Frequency.Minutely;
                } else if( value == "hourly" ){
                    this.Freq = Frequency.Hourly;
                } else if( value == "daily" ){
                    this.Freq = Frequency.Daily;
                } else if( value == "weekly" ){
                    this.Freq = Frequency.Weekly;
                } else if( value == "monthly" ){
                    this.Freq = Frequency.Monthly;
                } else if( value == "yearly" ){
                    this.Freq = Frequency.Yearly;
                }
            }

            // until
            key = "until";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                iCalDateTime datetime = new iCalDateTime( param.Value() );
                if (datetime.Time == null) {
                    this.Until = datetime.Date;
                } else  {
                    this.Until = datetime;
                }
            }

            // count
            key = "count";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Count = Int32.Parse( param.Value() );
            }

            // interval
            key = "interval";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Interval = Int32.Parse( param.Value() );
            }

            // bysec
            key = "bysecond";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.BySecList.Add( Int32.Parse( val ) );
                }
            }            

            // bymin
            key = "byminute";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.ByMinList.Add( Int32.Parse( val ) );
                }
            }            

            // byhour
            key = "byhour";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.ByHourList.Add( Int32.Parse( val ) );
                }
            }            

            // by day of week
            key = "byday";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    iCalDayOfWeek dow = new iCalDayOfWeek( val );
                    this.ByWeekDayList.Add( dow );
                }
            }            

            // month day
            key = "bymonthday";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.ByMonthDayList.Add( Int32.Parse( val ) );
                }
            }            
            
            // byyearday
            key = "byyearday";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.ByYearDayList.Add( Int32.Parse( val ) );
                }
            }            

            // byweekno
            key = "byweekno";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.ByWeeknoList.Add( Int32.Parse( val ) );
                }
            }            
            
            // bymonth
            key = "bymonth";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.ByMonthList.Add( Int32.Parse( val ) );
                }
            }            

            // bysetpos
            key = "bysetpos";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                foreach( String val in param.Values ){
                    this.BySetposList.Add( Int32.Parse( val ) );
                }
            }            
            
            // wkst
            key = "wkst";
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                String str1 = param.Value().ToLower();

                if( str1 == "su" ){
                    this.WeekStart = DayOfWeek.Sunday;
                } else if( str1 == "mo" ){
                    this.WeekStart = DayOfWeek.Monday;
                } else if( str1 == "tu" ){
                    this.WeekStart = DayOfWeek.Tuesday;
                } else if( str1 == "we" ){
                    this.WeekStart = DayOfWeek.Wednesday;
                } else if( str1 == "th" ){
                    this.WeekStart = DayOfWeek.Thursday;
                } else if( str1 == "fr" ){
                    this.WeekStart = DayOfWeek.Friday;
                } else if( str1 == "sa" ){
                    this.WeekStart = DayOfWeek.Saturday;
                }
            }
        }

        public override Object Clone()
        {
            iCalRecurrence ret = (iCalRecurrence)base.Clone();

            ret.Freq = this.Freq;
            if( this.Until != null ){
                ret.Until = (iCalTimeRelatedType)this.Until.Clone();
            }
            ret.Count = this.Count;

            ret.BySecList = new SortedSet<int>();
            foreach( int i in this.BySecList ){
                ret.BySecList.Add( i );
            }
            
            ret.ByMinList = new SortedSet<int>();
            foreach( int i in this.ByMinList ){
                ret.ByMinList.Add( i );
            }

            ret.ByHourList = new SortedSet<int>();
            foreach( int i in this.ByHourList ){
                ret.ByHourList.Add( i );
            }

            ret.ByWeekDayList = new SortedSet<iCalDayOfWeek>();
            foreach( iCalDayOfWeek dow in this.ByWeekDayList ){
                ret.ByWeekDayList.Add( (iCalDayOfWeek)dow.Clone() );
            }

            ret.ByMonthDayList = new SortedSet<int>();
            foreach( int i in this.ByMonthDayList ){
                ret.ByMonthDayList.Add( i );
            }

            ret.ByYearDayList = new SortedSet<int>();
            foreach( int i in this.ByYearDayList ){
                ret.ByYearDayList.Add( i );
            }

            ret.ByWeeknoList = new SortedSet<int>();
            foreach( int i in this.ByWeeknoList ){
                ret.ByWeeknoList.Add( i );
            }

            ret.ByMonthList = new SortedSet<int>();
            foreach( int i in this.ByMonthList ){
                ret.ByMonthList.Add( i );
            }

            ret.BySetposList = new SortedSet<int>();
            foreach( int i in this.BySetposList ){
                ret.BySetposList.Add( i );
            }
            
            ret.WeekStart = this.WeekStart;

            return ret;
        }

        public override String ToString()
        {
            String ret = "";
            if( this.Freq == Frequency.Secondly ){
                ret += "Secondly,";
            } else if( this.Freq == Frequency.Minutely ){
                ret += "Minutely,";
            } else if( this.Freq == Frequency.Hourly ){
                ret += "Hourly,";
            } else if( this.Freq == Frequency.Daily ){
                ret += "Daily,";
            } else if( this.Freq == Frequency.Weekly ){
                ret += "Weekly,";
            } else if( this.Freq == Frequency.Monthly ){
                ret += "Monthly,";
            } else if( this.Freq == Frequency.Yearly ){
                ret += "Yearly,";
            } else {
                ret += "??Unkown Frequency,";
            }

            ret += "Until:" + this.Until + ",";
            ret += "Count:" + this.Count + ",";
            ret += "Interval:" + this.Interval;
            return ret;
        }
    }

    public class iCalRecurrenceEnum : IEnumerator<iCalTimeRelatedType>
    {
        iCalTimeRelatedType OriginalDateTime;
        iCalRecurrence OriginalRec;
        iCalRecurrence Rec;

        bool First = true;
        int Count;

        IEnumerator<int> BySecEnum;
        bool BySecListReset = false;
        
        IEnumerator<int> ByMinEnum;
        bool ByMinListReset = false;
        
        IEnumerator<int> ByHourEnum;
        bool ByHourListReset = false;

        IEnumerator<iCalDayOfWeek> ByWeekDayEnum;
        bool ByWeekDayListReset = false;

        IEnumerator<int> ByMonthDayEnum;
        bool ByMonthDayListReset = false;

        IEnumerator<int> ByYearDayEnum;
        // bool ByYearDayListReset = false;
        
        IEnumerator<int> ByWeeknoEnum;
        // bool ByWeeknoListReset = false;

        IEnumerator<int> ByMonthEnum;
        bool ByMonthListReset = false;

        IEnumerator<int> BySetposEnum;

        SortedSet<int> AllDayList;
        SortedSet<int> AllMonthList;

        SortedSet<iCalTimeRelatedType> ListForSetpos;

        public iCalTimeRelatedType Current { get; set; }

        public iCalRecurrenceEnum( iCalTimeRelatedType original,
                                   iCalRecurrence recurrence )
        {
            this.OriginalDateTime = 
                (iCalTimeRelatedType)original.Clone();
            this.OriginalRec = recurrence;
            this.Rec = new iCalRecurrence();

            this.Reset();
        }

        Object IEnumerator.Current {
            get { return this.Current; }
        }
        
        iCalTimeRelatedType IEnumerator<iCalTimeRelatedType>.Current {
             get { return this.Current; }
        }

        public bool MoveNext()
        {
            if( this.OriginalRec.Count > 0 &&
                this.Count <= 0 ){
                return false;
            }

            bool found = false;
            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;

            if( freq == iCalRecurrence.Frequency.Secondly ){
                found = MoveNextSecondly();
            } else if( freq == iCalRecurrence.Frequency.Minutely ){
                found = MoveNextMinutely();
            } else if( freq == iCalRecurrence.Frequency.Hourly ){
                found = MoveNextHourly();
            } else if( freq == iCalRecurrence.Frequency.Daily ){
                found = MoveNextDaily();
            } else if( freq == iCalRecurrence.Frequency.Weekly ){
                found = MoveNextWeekly();
            } else if( freq == iCalRecurrence.Frequency.Monthly ){
                found = MoveNextMonthly();
            } else if( freq == iCalRecurrence.Frequency.Yearly ){
                found = MoveNextYearly();
            }

            if( found ){
                // until date will be checked in iCalRecurrenceRule class
                // if( this.OriginalRec.Until != null &&
                //     this.OriginalRec.Until < this.Current ){
                //     return false;
                // }
                if( this.OriginalRec.Count > 0 ){
                    this.Count --;
                }
                return true;
            }
            return false;
        }

        public bool MoveNextSecondly()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return false;
            } else {
                datetime = (iCalDateTime)datetime.Clone();
            }

            DateTime systemDateTime = (DateTime)datetime;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;

            for( int i = 0; i < max; i++ ){
                systemDateTime = systemDateTime.AddSeconds( interval );
                this.Current = new iCalDateTime( systemDateTime );

                if( this.Current == this.OriginalDateTime ){
                    continue;
                }

                if( CheckBySecList() == false ) {
                    continue;
                }
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                if( CheckByMonthDayList() == false ) {
                    continue;
                }
                if( CheckByYearDayList() == false ) {
                    continue;
                }
                // if( CheckByWeeknoList() == false ) {
                //     continue;
                // }
                if( CheckByMonthList() == false ) {
                    continue;
                }

                return true;
            }
            return false;
        }

        public bool MoveNextMinutely()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return false;
            } else {
                datetime = (iCalDateTime)datetime.Clone();
            }

            DateTime systemDateTime = (DateTime)datetime;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;

            for( int i = 0; i < max; i++ ){
                if( this.Rec.BySetposList.Count > 0 ){
                    if( ! this.MoveBySetPos() ){
                        systemDateTime = systemDateTime.AddMinutes( interval );
                        this.Current = new iCalDateTime( systemDateTime );
                        this.ResetBySetposList();
                        this.MoveBySetPos();
                    }

                } else {
                    if( ! this.MoveBySecList() ){
                        systemDateTime = systemDateTime.AddMinutes( interval );
                        this.Current = new iCalDateTime( systemDateTime );
                        this.ResetBySecList();
                        this.MoveBySecList();
                    }
                }

                if( this.Current == this.OriginalDateTime ){
                    continue;
                }
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                if( CheckByMonthDayList() == false ) {
                    continue;
                }
                if( CheckByYearDayList() == false ) {
                    continue;
                }
                // if( CheckByWeeknoList() == false ) {
                //     continue;
                // }
                if( CheckByMonthList() == false ) {
                    continue;
                }
                
                // this.Current = new iCalDateTime( systemDateTime );

                return true;
            }            
            return false;
        }

        public void ResetBySetposListMinutely()
        {
            this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();

            if( this.OriginalRec.BySetposList.Count == 0 ){
                return;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return;
            }

            this.ResetBySecList();
            while( this.BySecEnum.MoveNext() ){
                datetime = (iCalDateTime)datetime.Clone();
                datetime.Time.Second = this.BySecEnum.Current;
                this.ListForSetpos.Add( datetime );
            }
            // this.BySecEnum.Reset();
        }

        public bool MoveNextHourly()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return false;
            }

            DateTime systemDateTime = (DateTime)datetime;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;
            for( int i = 0; i < max; i++ ){
                if( this.Rec.BySetposList.Count > 0 ){
                    if( ! this.MoveBySetPos() ){
                        systemDateTime = systemDateTime.AddHours( interval );
                        this.Current = new iCalDateTime( systemDateTime );
                        ResetBySetposList();
                        this.MoveBySetPos();
                    }
                } else if( this.First == true ){
                    this.MoveByMinList();
                    this.First = false;
                } else {
                    bool ret;
                    ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        systemDateTime = systemDateTime.AddHours( interval );
                        this.Current = new iCalDateTime( systemDateTime );
                        this.ResetByMinList();
                        this.MoveByMinList();
                    }
                }

                if( this.Current == this.OriginalDateTime ){
                    continue;
                }
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                if( CheckByMonthDayList() == false ) {
                    continue;
                }
                if( CheckByYearDayList() == false ) {
                    continue;
                }
                // if( CheckByWeeknoList() == false ) {
                //     continue;
                // }
                if( CheckByMonthList() == false ) {
                    continue;
                }
                
                // this.Current = new iCalDateTime( systemDateTime );

                return true;
            }
            return false;
        }

        public void ResetBySetposListHourly()
        {
            this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();

            if( this.OriginalRec.BySetposList.Count == 0 ){
                return;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return;
            }

            iCalTimeRelatedType Original = 
                (iCalTimeRelatedType)this.Current.Clone();
            bool OriginalFirst = this.First;
                
            while( true ){
                if( this.First == true ){
                    this.MoveByMinList();
                } else {
                    bool ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        break;
                    }
                }
                if( this.CheckByMinList() == false ) {
                    continue;
                }
                this.ListForSetpos.Add( (iCalTimeRelatedType)
                                        this.Current.Clone() );
            }

            this.First = OriginalFirst;
            this.Current = Original;
            this.ResetByMinList();
            this.ResetBySecList();
        }

        public bool MoveNextDaily()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            } else {
                date = (iCalDate)date.Clone();
            }

            DateTime systemDateTime = (DateTime)date;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;
            for( int i = 0; i < max; i++ ){
                if( this.Rec.BySetposList.Count > 0 ){
                    if( ! this.MoveBySetPos() ){
                        systemDateTime = systemDateTime.AddDays( interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        ResetBySetposList();
                        this.MoveBySetPos();
                    }
                } else if( this.First == true ) {
                    this.MoveByHourList();
                    this.First = false;
                } else {
                    bool ret;
                    ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        systemDateTime = systemDateTime.AddDays( interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        this.ResetByHourList();
                        this.MoveByHourList();
                    }
                }

                if( this.Current == this.OriginalDateTime ){
                    continue;
                }
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                if( CheckByMonthDayList() == false ) {
                    continue;
                }
                // if( CheckByYearDayList() == false ) {
                //     continue;
                // }
                // if( CheckByWeeknoList() == false ) {
                //     continue;
                // }
                if( CheckByMonthList() == false ) {
                    continue;
                }
                
                // this.Current = new iCalDateTime( systemDateTime );

                return true;
            }
            return false;
        }

        public void ResetBySetposListDaily()
        {
            this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();

            if( this.OriginalRec.BySetposList.Count == 0 ){
                return;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                date = datetime.Date;
            }
            if( date == null ){
                return;
            }

            iCalTimeRelatedType Original = 
                (iCalTimeRelatedType)this.Current.Clone();
            bool OriginalFirst = this.First;

            while( true ){
                if( this.First == true ){
                    this.MoveByHourList();
                    this.First = false;
                } else {
                    bool ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    
                    if( ret == false ){
                        break;
                    }
                }
                if( this.CheckByMinList() == false ){
                    continue;
                }
                if( this.CheckByHourList() == false ){
                    continue;
                }
                this.ListForSetpos.Add( (iCalTimeRelatedType)
                                        this.Current.Clone() );
            }
            this.First = OriginalFirst;
            this.Current = Original;
            this.ResetByMinList();
            this.ResetBySecList();
            this.ResetByHourList();
        }

        public bool MoveNextWeekly()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            } else {
                date = (iCalDate)date.Clone();
            }
            DateTime systemDateTime = (DateTime)date;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;
            for( int i = 0; i < max; i++ ){
                if( this.Rec.BySetposList.Count > 0 ){
                    if( ! this.MoveBySetPos() ){
                        DayOfWeek OriginalDayOfWeek
                            = this.OriginalDateTime.GetDayOfWeek();
                        while( systemDateTime.DayOfWeek != OriginalDayOfWeek )
                        {
                            systemDateTime = systemDateTime.AddDays( -1 );
                        }
                        systemDateTime = systemDateTime.AddDays( 7 * interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        ResetBySetposList();
                        this.MoveBySetPos();
                    }
                } else if( this.First == true ){
                    this.MoveByWeekDayList();
                    this.First = false;
                } else {
                    bool ret;
                    ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        ret = this.MoveByWeekDayList();
                    }
                    if( ret == false ){
                        DayOfWeek OriginalDayOfWeek
                            = this.OriginalDateTime.GetDayOfWeek();
                        while( systemDateTime.DayOfWeek != OriginalDayOfWeek )
                        {
                            systemDateTime = systemDateTime.AddDays( -1 );
                        }
                        
                        systemDateTime = systemDateTime.AddDays( 7 * interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        this.ResetByWeekDayList();
                        this.MoveByWeekDayList();
                    }
                }
                if( this.Current == this.OriginalDateTime ){
                    continue;
                }
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                // if( CheckByMonthDayList() == false ) {
                //     continue;
                // }
                // if( CheckByYearDayList() == false ) {
                //     continue;
                // }
                // if( CheckByWeeknoList() == false ) {
                //     continue;
                // }
                if( CheckByMonthList() == false ) {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void ResetBySetposListWeekly()
        {
            this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();

            if( this.OriginalRec.BySetposList.Count == 0 ){
                return;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                date = datetime.Date;
            }
            if( date == null ){
                return;
            }

            iCalTimeRelatedType Original = 
                (iCalTimeRelatedType)this.Current.Clone();
            bool OriginalFirst = this.First;

            while( true ){
                if( this.First == true ){
                    this.MoveByWeekDayList();
                    this.First = false;
                } else {
                    bool ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        ret = this.MoveByWeekDayList();
                    }
                    if( ret == false ){
                        break;
                    }
                }
                if( this.CheckByMinList() == false ){
                    continue;
                }
                if( this.CheckByHourList() == false ){
                    continue;
                }
                if( this.CheckByWeekDayList() == false ){
                    continue;
                }
                this.ListForSetpos.Add( (iCalTimeRelatedType)
                                        this.Current.Clone() );
            }
            this.First = OriginalFirst;
            this.Current = Original;
            this.ResetByMinList();
            this.ResetBySecList();
            this.ResetByHourList();
            this.ResetByWeekDayList();
        }

        public bool MoveNextMonthly()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            } else {
                date = (iCalDate)date.Clone();
            }

            DateTime systemDateTime = (DateTime)date;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;
            for( int i = 0; i < max; i++ ){
                if( this.Rec.BySetposList.Count > 0 ){
                    if( ! this.MoveBySetPos() ){
                        systemDateTime = systemDateTime.AddMonths( interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        ResetBySetposList();
                        this.MoveBySetPos();
                    }
                } else if( this.First == true ){
                    this.MoveByMonthDayList();
                    this.First = false;
                } else {
                    bool ret;
                    ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        ret = this.MoveByMonthDayList();
                    }
                    if( ret == false ){
                        systemDateTime = systemDateTime.AddMonths( interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        this.ResetByMonthDayList();
                        this.MoveByMonthDayList();
                    }
                }
                if( this.Current == this.OriginalDateTime ){
                    continue;
                }
                        
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                if( CheckByMonthDayList() == false ) {
                    continue;
                }
                // if( CheckByYearDayList() == false ) {
                //     continue;
                // }
                // if( CheckByWeeknoList() == false ) {
                //     continue;
                // }
                if( CheckByMonthList() == false ) {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void ResetBySetposListMonthly()
        {
            this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();

            if( this.OriginalRec.BySetposList.Count == 0 ){
                return;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                date = datetime.Date;
            }
            if( date == null ){
                return;
            }

            iCalTimeRelatedType Original = 
                (iCalTimeRelatedType)this.Current.Clone();

            bool OriginalFirst = this.First;
            
            while( true ){
                if( this.First == true ){
                    this.MoveByMonthDayList();
                    this.First = false;
                } else {
                    bool ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        ret = this.MoveByMonthDayList();
                    }
                    if( ret == false ){
                        break;
                    }
                }
                if( this.CheckByMinList() == false ){
                    continue;
                }
                if( this.CheckByHourList() == false ){
                    continue;
                }
                if( this.CheckByWeekDayList() == false ){
                    continue;
                }
                if( this.CheckByMonthDayList() == false ){
                    continue;
                }

                this.ListForSetpos.Add( (iCalTimeRelatedType)
                                        this.Current.Clone() );
            }

            this.First = OriginalFirst;
            this.Current = Original;
            this.ResetByMinList();
            this.ResetBySecList();
            this.ResetByHourList();
            this.ResetByWeekDayList();
            this.ResetByMonthDayList();
        }

        public bool MoveNextYearly()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            } else {
                date = (iCalDate)date.Clone();
            }

            DateTime systemDateTime = (DateTime)date;
            int interval = this.OriginalRec.Interval;
            int max = 366 * 24 * 60 * 60;
            for( int i = 0; i < max; i++ ){
                if( this.Rec.BySetposList.Count > 0 ){
                    if( ! this.MoveBySetPos() ){
                        systemDateTime = systemDateTime.AddYears( interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        ResetBySetposList();
                        this.MoveBySetPos();
                    }
                } else if( this.First == true ){
                    this.MoveByMonthList();
                    this.First = false;
                } else {
                    bool ret;
                    ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        ret = this.MoveByMonthDayList();
                    }
                    if( ret == false ){
                        ret = this.MoveByMonthList();
                    }
                    if( ret == false ){
                        systemDateTime = systemDateTime.AddYears( interval );
                        date = new iCalDate( systemDateTime );
                        if( datetime != null ){
                            datetime.Date = date;
                            this.Current = datetime;
                        } else {
                            this.Current = date;
                        }
                        this.ResetByMonthList();
                        this.MoveByMonthList();
                    }
                }

                if( this.Current == this.OriginalDateTime ){
                    continue;
                }
                if( CheckByMinList() == false ) {
                    continue;
                }
                if( CheckByHourList() == false ) {
                    continue;
                }
                if( CheckByWeekDayList() == false ) {
                    continue;
                }
                if( CheckByMonthDayList() == false ) {
                    continue;
                }
                if( CheckByYearDayList() == false ) {
                    continue;
                }
                if( CheckByWeeknoList() == false ) {
                    continue;
                }
                if( CheckByMonthList() == false ) {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void ResetBySetposListYearly()
        {
            this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();

            if( this.OriginalRec.BySetposList.Count == 0 ){
                return;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                date = datetime.Date;
            }
            if( date == null ){
                return;
            }

            iCalTimeRelatedType Original = 
                (iCalTimeRelatedType)this.Current.Clone();
            bool OriginalFirst = this.First;

            while( true ){
                if( this.First ){
                    this.MoveByMonthList();
                    this.First = false;
                } else {
                    bool ret = this.MoveBySecList();
                    if( ret == false ){
                        ret = this.MoveByMinList();
                    }
                    if( ret == false ){
                        ret = this.MoveByHourList();
                    }
                    if( ret == false ){
                        ret = this.MoveByMonthDayList();
                    }
                    if( ret == false ){
                        ret = this.MoveByMonthList();
                    }
                    if( ret == false ){
                        break;
                    }
                    if( this.CheckByMinList() == false ){
                        continue;
                    }
                    if( this.CheckByHourList() == false ){
                        continue;
                    }
                    if( this.CheckByWeekDayList() == false ){
                        continue;
                    }
                    if( this.CheckByMonthDayList() == false ){
                        continue;
                    }
                    if( CheckByYearDayList() == false ) {
                        continue;
                    }
                    if( CheckByWeeknoList() == false ) {
                        continue;
                    }
                    if( CheckByMonthList() == false ) {
                        continue;
                    }
                }

                this.ListForSetpos.Add( (iCalTimeRelatedType)
                                        this.Current.Clone() );
            }

            this.First = OriginalFirst;
            this.Current = Original;
            this.ResetAllList();
        }

        public bool MoveBySetPos()
        {
            if( this.BySetposEnum.MoveNext() ){
                int setpos = this.BySetposEnum.Current;
                iCalTimeRelatedType time = this.GetDateTimeBySetPos( setpos );
                this.Current = time;

                return true;
            }
            return false;
        }

        public bool MoveBySecList()
        {
            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return false;
            }

            while( this.BySecEnum.MoveNext() ){
                if( this.BySecListReset == true ){
                    this.BySecListReset = false;
                } else if( datetime.Time.Second == this.BySecEnum.Current ){
                    continue;
                }
                datetime.Time.Second = this.BySecEnum.Current;
                this.Current = (iCalDateTime)datetime.Clone();
                return true;
                // break;
            }
            return false;
        }

        public bool MoveByMinList()
        {
            bool ret = false;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return false;
            }
            
            while( this.ByMinEnum.MoveNext() ){
                if( this.ByMinListReset == true ){
                    this.ByMinListReset = false;
                } else if( datetime.Time.Minute == this.ByMinEnum.Current ){
                    continue;
                }
                datetime.Time.Minute = this.ByMinEnum.Current;
                this.Current = (iCalDateTime)datetime.Clone();
                ret = true;
                break;
            }
            ResetBySecList();
            this.MoveBySecList();

            return ret;
        }

        public bool MoveByHourList()
        {
            bool ret = false;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            if( datetime == null ){
                return false;
            }

            while( this.ByHourEnum.MoveNext() ){
                if( this.ByHourListReset == true ){
                    this.ByHourListReset = false;
                } else if( datetime.Time.Hour == this.ByHourEnum.Current ){
                    continue;
                }
                datetime.Time.Hour = this.ByHourEnum.Current;
                this.Current = (iCalDateTime)datetime.Clone();
                ret = true;
                break;
            }
            this.ResetByMinList();
            this.MoveByMinList();

            return ret;
        }

        public bool MoveByWeekDayList()
        {
            bool ret = false;

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            }
            DateTime systemDateTime = (DateTime)date;
            
            DayOfWeek weekStart = this.OriginalRec.WeekStart;
            iCalDayOfWeek dow = new iCalDayOfWeek();

            while( true ){
                // after week day list reset, must check current date,
                // without moving to next day
                if( this.ByWeekDayListReset ){
                    this.ByWeekDayListReset = false;
                }  else {
                    systemDateTime = systemDateTime.AddDays( 1 );
                }

                if( weekStart == systemDateTime.DayOfWeek ){
                    break;
                }
                
                dow.NumOfWeek = 0;
                dow.DayOfWeek = systemDateTime.DayOfWeek;
                if( this.Rec.ByWeekDayList.Contains( dow ) ){
                    date = new iCalDate( systemDateTime );
                    if( datetime != null ){
                        datetime.Date = date;
                        this.Current = datetime;
                    } else {
                        this.Current = date;
                    }
                    ret = true;
                    break;
                }
            }
            ResetByHourList();
            this.MoveByHourList();

            return ret;
        }

        public bool MoveByMonthDayList(){
            bool ret = false;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            }

            while( this.ByMonthDayEnum.MoveNext() ){
                if( this.ByMonthDayListReset == true ){
                    this.ByMonthDayListReset = false;
                } else if( date.Day == this.ByMonthDayEnum.Current ){
                    continue;
                }
                date.Day = this.ByMonthDayEnum.Current;
                if( datetime != null ){
                    datetime.Date = date;
                    this.Current = datetime;
                } else {
                    this.Current = date;
                }
                ret = true;
                break;
            }
            this.ResetByHourList();
            this.MoveByHourList();

            return ret;
        }

        public bool MoveByMonthList(){
            bool ret = false;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                datetime = (iCalDateTime)datetime.Clone();
                date = datetime.Date;
            }
            if( date == null ){
                return false;
            }

            while( this.ByMonthEnum.MoveNext() ){
                if( this.ByMonthListReset == true ){
                    this.ByMonthListReset = false;
                } else if( date.Month == this.ByMonthEnum.Current ){
                    continue;
                }
                date.Month = this.ByMonthEnum.Current;
                if( datetime != null ){
                    datetime.Date = date;
                    this.Current = datetime;
                } else {
                    this.Current = date;
                }
                ret = true;
                break;
            }
            this.ResetByMonthDayList();
            this.MoveByMonthDayList();

            return ret;
        }

        public void Reset(){
            this.Count = this.OriginalRec.Count-1;
            this.Current = (iCalTimeRelatedType)this.OriginalDateTime.Clone();
            ResetAllList();
            this.First = true;
        }

        void ResetAllList()
        {
            this.ResetBySecList();
            this.ResetByMinList();
            this.ResetByHourList();
            this.ResetByMonthDayList();
            this.ResetByWeekDayList();
            this.ResetByYearDayList();
            this.ResetByWeeknoList();
            this.ResetByMonthList();
            this.ResetBySetposList();
        }

        void ResetBySecList(){

            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;
            int interval = this.OriginalRec.Interval;

            this.Rec.BySecList = new SortedSet<int>();

            foreach( int i in this.OriginalRec.BySecList ){
                int i2 = i;
                if( i < 0 ){
                    i2 = 60 + i;
                }
                this.Rec.BySecList.Add( i2 );
            }
            this.BySecEnum = this.Rec.BySecList.GetEnumerator();
            this.BySecListReset = true;
        }

        bool CheckBySecList(){
            if( this.Rec.BySecList.Count == 0 ){
                return true;
            } else {
                iCalDateTime datetime = this.Current as iCalDateTime;
                if( datetime == null ){
                    return true;
                } else {
                    int second = datetime.Time.Second;
                    if( this.Rec.BySecList.Contains( second ) ){
                        return true;
                    }
                }
            }
            return false;
        }

        void ResetByMinList(){

            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;
            int interval = this.OriginalRec.Interval;

            this.Rec.ByMinList = new SortedSet<int>();

            foreach( int i in this.OriginalRec.ByMinList ){
                int i2 = i;
                if( i < 0 ){
                    i2 = 60 + i;
                }
                this.Rec.ByMinList.Add( i2 );
            }
            this.ByMinEnum = this.Rec.ByMinList.GetEnumerator();
            this.ByMinListReset = true;
        }

        bool CheckByMinList(){
            if( this.Rec.ByMinList.Count == 0 ){
                return true;
            } else {
                iCalDateTime datetime = this.Current as iCalDateTime;
                if( datetime == null ){
                    return true;
                } else {
                    int minute = datetime.Time.Minute;
                    if( this.Rec.ByMinList.Contains( minute ) ){
                        return true;
                    }
                }
            }
            return false;
        }

        void ResetByHourList(){

            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;
            int interval = this.OriginalRec.Interval;

            this.Rec.ByHourList = new SortedSet<int>();

            foreach( int i in this.OriginalRec.ByHourList ){
                int i2 = i;
                if( i < 0 ){
                    i2 = 24 + i;
                }
                this.Rec.ByHourList.Add( i2 );
            }
            this.ByHourEnum = this.Rec.ByHourList.GetEnumerator();
            this.ByHourListReset = true;
        }

        bool CheckByHourList(){
            if( this.Rec.ByHourList.Count == 0 ){
                return true;
            } else {
                iCalDateTime datetime = this.Current as iCalDateTime;
                if( datetime == null ){
                    return true;
                } else {
                    int hour = datetime.Time.Hour;
                    if( this.Rec.ByHourList.Contains( hour ) ){
                        return true;
                    }
                }
            }
            return false;
        }

        void ResetByMonthDayList(){
            int year = 0;
            int month = 0;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;

            if( datetime != null && datetime.Date != null ){
                year = datetime.Date.Year;
                month = datetime.Date.Month;
            } else if( date != null ){
                year = date.Year;
                month = date.Month;
            }
            if( year == 0 || month == 0 ){
                throw new ArgumentException( "not iCalDateTime nor iCalDate" );
            }

            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;

            Calendar systemCalendar = new GregorianCalendar();
            int lastDay = systemCalendar.GetDaysInMonth( year, month );

            if( freq == iCalRecurrence.Frequency.Monthly ||
                freq == iCalRecurrence.Frequency.Yearly ){
                this.AllDayList = new SortedSet<int>();
            }

            if( this.OriginalRec.ByMonthList != null &&
                this.OriginalRec.ByMonthList.Contains( month ) ){

                this.Rec.ByMonthDayList = new SortedSet<int>();
                foreach( int i in this.OriginalRec.ByMonthDayList ){
                    int i2 = i;
                    if( i < 0 ){
                        i2 = lastDay + i + 1;
                    }
                    this.Rec.ByMonthDayList.Add( i2 );
                    if( freq == iCalRecurrence.Frequency.Monthly ||
                        freq == iCalRecurrence.Frequency.Yearly ){
                        this.AllDayList.Add( i2 );
                    }
                }
            }

            if( freq == iCalRecurrence.Frequency.Monthly ||
                freq == iCalRecurrence.Frequency.Yearly ){
                this.ResetByWeekDayList();
                foreach( iCalDayOfWeek dow in this.Rec.ByWeekDayList ){
                    if( freq == iCalRecurrence.Frequency.Monthly ||
                        this.Rec.ByMonthList.Count > 0 ){
                        date = dow.GetFirstDate( year, month );
                        if (date != null) {
                            if (dow.NumOfWeek == 0) {
                                int day = date.Day;
                                while (day <= lastDay) {
                                    this.AllDayList.Add(day);
                                    day += 7;
                                }
                            } else {
                                this.AllDayList.Add(date.Day);
                            }
                        }
                    } else {
                        date = dow.GetFirstDate( year );
                        if( dow.NumOfWeek == 0 ){
                            DateTime systemDateTime = (DateTime)date;
                            while( systemDateTime.Year == year ){
                                if( systemDateTime.Month == month ){
                                    this.AllDayList.Add( systemDateTime.Day );
                                }
                                systemDateTime = systemDateTime.AddDays( 7 );
                            }
                        } else {
                            if( date.Year == year && date.Month == month ){
                                this.AllDayList.Add( date.Day );
                            }
                        }
                    }
                }
            }

            if( freq == iCalRecurrence.Frequency.Yearly ){
                this.ResetByYearDayList();
                foreach( int i in this.Rec.ByYearDayList ){
                    DateTime systemDateTime = new DateTime( year, 1, 1 );
                    systemDateTime = systemDateTime.AddDays( i - 1 );
                    if( systemDateTime.Year == year &&
                        systemDateTime.Month == month ){
                        this.AllDayList.Add( systemDateTime.Day );
                    }
                }

                this.ResetByWeeknoList();
                foreach( int i in this.Rec.ByWeeknoList ){
                    iCalDayOfWeek dow = new iCalDayOfWeek();
                    dow.NumOfWeek = i;
                    dow.DayOfWeek = DayOfWeek.Thursday;

                    DayOfWeek weekStart = this.OriginalRec.WeekStart;
                    DateTime from, to;

                    // previous year
                    date = dow.GetFirstDate( year -1 );
                    from = (DateTime)date;
                    while( from.DayOfWeek != weekStart ){
                        from = from.AddDays( -1 );
                    }
                    to = from.AddDays( 1 );
                    while( to.DayOfWeek != weekStart ){
                        to = to.AddDays( 1 );
                    }
                    for( DateTime dt = from; dt < to; dt = dt.AddDays( 1 ) ){
                        if( dt.Year == year && dt.Month == month ){
                            this.AllDayList.Add( dt.Day );
                        }
                    }

                    // current year
                    date = dow.GetFirstDate( year );
                    from = (DateTime)date;
                    while( from.DayOfWeek != weekStart ){
                        from = from.AddDays( -1 );
                    }
                    to = from.AddDays( 1 );
                    while( to.DayOfWeek != weekStart ){
                        to = to.AddDays( 1 );
                    }
                    for( DateTime dt = from; dt < to; dt = dt.AddDays( 1 ) ){
                        if( dt.Year == year && dt.Month == month ){
                            this.AllDayList.Add( dt.Day );
                        }
                    }

                    // next year
                    date = dow.GetFirstDate( year + 1 );
                    from = (DateTime)date;
                    while( from.DayOfWeek != weekStart ){
                        from = from.AddDays( -1 );
                    }
                    to = from.AddDays( 1 );
                    while( to.DayOfWeek != weekStart ){
                        to = to.AddDays( 1 );
                    }
                    for( DateTime dt = from; dt < to; dt = dt.AddDays( 1 ) ){
                        if( dt.Year == year && dt.Month == month ){
                            this.AllDayList.Add( dt.Day );
                        }
                    }
                }
            }

            if( freq == iCalRecurrence.Frequency.Monthly ||
                freq == iCalRecurrence.Frequency.Yearly ){
                this.ByMonthDayEnum = this.AllDayList.GetEnumerator();
            } else {
                this.ByMonthDayEnum = this.Rec.ByMonthDayList.GetEnumerator();
            }
            this.ByMonthDayListReset = true;
        }

        bool CheckByMonthDayList(){
            if( this.Rec.ByMonthDayList.Count == 0 ){
                return true;
            } else {
                iCalDateTime datetime = this.Current as iCalDateTime;
                iCalDate date = this.Current as iCalDate;
                if( datetime != null ){
                    date = datetime.Date;
                }
                if( date == null ){
                    return true;
                } else {
                    int monthday = date.Day;
                    if( this.Rec.ByMonthDayList.Contains( monthday ) ){
                        return true;
                    }
                }
            }
            return false;
        }

        void ResetByWeekDayList(){
            int year = 0;
            int month = 0;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;

            if( datetime != null && datetime.Date != null ){
                year = datetime.Date.Year;
                month = datetime.Date.Month;
            } else if( date != null ){
                year = date.Year;
                month = date.Month;
            }
            if( year == 0 || month == 0 ){
                throw new ArgumentException( "not iCalDateTime nor iCalDate" );
            }

            this.Rec.ByWeekDayList = new SortedSet<iCalDayOfWeek>();
            foreach( iCalDayOfWeek dow in this.OriginalRec.ByWeekDayList ){
                iCalDayOfWeek dow2 = (iCalDayOfWeek)dow.Clone();
                if( dow2.IsMinus == true ){

                    // Calendar systemCalendar = new GregorianCalendar();

                    // The BYDAY rule part MUST NOT be specified with a numeric
                    // value when the FREQ rule part is not set to
                    // MONTHLY or YEARLY.
                    // The numeric value in a BYDAY rule part with the FREQ
                    // rule part set to YEARLY corresponds to an offset within
                    // the month when the BYMONTH rule part is present.
                    if( this.OriginalRec.Freq == iCalRecurrence.Frequency.Yearly
                        && this.OriginalRec.ByMonthList.Count == 0 ){

                        date = dow2.GetFirstDate( year );
                        DateTime systemDateTime = (DateTime)date;
                        // int lastDay = systemCalendar.GetDaysInYear( year );
                        // int count = ( lastDay - systemDateTime.DayOfYear ) / 7;
                        // count = count + 1;

                        // dow2.NumOfWeek = count + dow2.NumOfWeek + 1;
                        dow2.NumOfWeek = systemDateTime.DayOfYear / 7;
                        if (systemDateTime.DayOfYear % 7 > 0) {
                            dow2.NumOfWeek++;
                        }
                        dow2.IsMinus = false;
                    } else {
                        date = dow2.GetFirstDate( year, month );
                        // int lastDay = systemCalendar.GetDaysInMonth( year, month );
                        // int count = ( lastDay - date.Day ) / 7 + 1;

                        dow2.NumOfWeek = date.Day / 7;
                        if( date.Day % 7 > 0){
                            dow2.NumOfWeek++;
                        }
                        dow2.IsMinus = false;
                    }
                }
                this.Rec.ByWeekDayList.Add( dow2 );
            }
            this.ByWeekDayEnum = this.Rec.ByWeekDayList.GetEnumerator();
            this.ByWeekDayListReset = true;
        }

        bool CheckByWeekDayList(){
            if( this.Rec.ByWeekDayList.Count == 0 ){
                return true;
            }
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                date = datetime.Date;
            }
            if( date == null ){
                return true;
            }

            DateTime systemDateTime = (DateTime)date;
            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;

            bool offsetOfYear = false;
            bool offsetOfMonth = false;

            if( freq == iCalRecurrence.Frequency.Yearly ){
                if( this.Rec.ByMonthList.Count > 0 ){
                    offsetOfMonth = true;
                } else {
                    offsetOfYear = true;
                }
            } else if( freq == iCalRecurrence.Frequency.Monthly ){
                offsetOfMonth = true;
            }


            foreach( iCalDayOfWeek dow in this.Rec.ByWeekDayList ){
                if( dow.DayOfWeek != systemDateTime.DayOfWeek ){
                    continue;
                }
                if( dow.NumOfWeek == 0 ){
                    return true;
                }
                if( offsetOfYear == true ){
                    date = dow.GetFirstDate( systemDateTime.Year );
                    DateTime systemDateTime2 = (DateTime)date;
                    if( systemDateTime2 == systemDateTime ){
                        return true;
                    }
                } else if( offsetOfMonth == true ){
                    date = dow.GetFirstDate( systemDateTime.Year,
                                             systemDateTime.Month );
                    DateTime systemDateTime2 = (DateTime)date;
                    if( systemDateTime2 == systemDateTime ){
                        return true;
                    }
                } else {
                    return true;
                }
            }
            return false;
        }

        void ResetByYearDayList(){
            int year = 0;
            int month = 0;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;

            if( datetime != null && datetime.Date != null ){
                year = datetime.Date.Year;
                month = datetime.Date.Month;
            } else if( date != null ){
                year = date.Year;
                month = date.Month;
            }
            if( year == 0 || month == 0 ){
                throw new ArgumentException( "not iCalDateTime nor iCalDate" );
            }

            Calendar systemCalendar = new GregorianCalendar();
            int lastDay = systemCalendar.GetDaysInYear( year );

            this.Rec.ByYearDayList = new SortedSet<int>();
            foreach( int i in this.OriginalRec.ByYearDayList ){
                int i2 = i;
                if( i < 0 ){
                    i2 = lastDay + i + 1;
                }
                this.Rec.ByYearDayList.Add( i2 );
            }
            this.ByYearDayEnum = this.Rec.ByYearDayList.GetEnumerator();
            // this.ByYearDayListReset = true;
        }

        bool CheckByYearDayList(){
            if( this.Rec.ByYearDayList.Count == 0 ){
                return true;
            } else {
                iCalDateTime datetime = this.Current as iCalDateTime;
                iCalDate date = this.Current as iCalDate;
                if( datetime != null ){
                    date = datetime.Date;
                }
                if( date == null ){
                    return true;
                } else {
                    DateTime systemDateTime = (DateTime)date;

                    if( this.Rec.ByYearDayList.Contains( systemDateTime.DayOfYear ) ) {
                        return true;
                    }
                }
            }
            return false;
        }

        void ResetByWeeknoList(){
            int year = 0;
            
            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;

            if( datetime != null && datetime.Date != null ){
                year = datetime.Date.Year;
            } else if( date != null ){
                year = date.Year;
            }
            if( year == 0 ){
                throw new ArgumentException( "not iCalDateTime nor iCalDate" );
            }

            Calendar systemCalendar = new GregorianCalendar();
            int lastDay = systemCalendar.GetDaysInYear( year );
            iCalDayOfWeek dow = new iCalDayOfWeek();
            // Thursday is center of week where to count week number
            dow.DayOfWeek = DayOfWeek.Thursday;
            dow.NumOfWeek = 0;
            date = dow.GetFirstDate( year );
            DateTime systemDateTime2 = (DateTime)date;
            int count = (lastDay - systemDateTime2.DayOfYear)/7 + 1;

            this.Rec.ByWeeknoList = new SortedSet<int>();
            foreach( int i in this.OriginalRec.ByWeeknoList ){
                int num = i;
                if( i < 0 ){
                    num = count + i + 1;
                }

                this.Rec.ByWeeknoList.Add( i );

                // // thursday;
                // DateTime systemDateTime = (DateTime)ret;
                // this.ByMonthDayList.Add( systemDateTime.Day );
                
                // DateTime systemDateTime2;
                // DayOfWeek weekStart = this.OriginalRec.WeekStart;

                // // Friday;
                // if( weekStart == DayOfWeek.Friday ){
                //     systemDateTime2 = systemDateTime.AddDays( -7 + 1 );
                // } else {
                //     systemDateTime2 = systemDateTime.AddDays( 1 );
                // }
                // if( systemDateTime2.Year == year &&
                //     systemDateTime2.Month == month ){
                //     this.ByMonthDayList.Add( systemDateTime2.Day );
                // }

                // // Saturday
                // if( weekStart == DayOfWeek.Friday ||
                //     weekStart == DayOfWeek.Saturday ){
                //     systemDateTime2 = systemDateTime.AddDays( -7 + 2 );
                // } else {
                //     systemDateTime2 = systemDateTime.AddDays( 2 );
                // }
                // if( systemDateTime2.Year == year &&
                //     systemDateTime2.Month == month ){
                //     this.ByMonthDayList.Add( systemDateTime2.Day );
                // }
                
                // // Sunday
                // if( weekStart == DayOfWeek.Friday ||
                //     weekStart == DayOfWeek.Saturday ){
                //     systemDateTime2 = systemDateTime.AddDays( -7 + 3 );
                // } else {
                //     systemDateTime2 = systemDateTime.AddDays( 3 );
                // }
                // if( systemDateTime2.Year == year &&
                //     systemDateTime2.Month == month ){
                //     this.ByMonthDayList.Add( systemDateTime2.Day );
                // }

                // // Monday
                // if( weekStart == DayOfWeek.Sunday ||
                //     weekStart == DayOfWeek.Monday ||
                //     weekStart == DayOfWeek.Saturday ){
                //     systemDateTime2 = systemDateTime.AddDays( -7 + 4 );
                // } else {
                //     systemDateTime2 = systemDateTime.AddDays( 4 );
                // }
                // if( systemDateTime2.Year == year &&
                //     systemDateTime2.Month == month ){
                //     this.ByMonthDayList.Add( systemDateTime2.Day );
                // }

                // // Tuesday;
                // if( weekStart == DayOfWeek.Wednesday ||
                //     weekStart == DayOfWeek.Thursday ){
                //     systemDateTime2 = systemDateTime.AddDays( 5 );
                // } else {
                //     systemDateTime2 = systemDateTime.AddDays( -7 + 5 );
                // }
                // if( systemDateTime2.Year == year &&
                //     systemDateTime2.Month == month ){
                //     this.ByMonthDayList.Add( systemDateTime2.Day );
                // }

                // // Wendsday;
                // if( weekStart == DayOfWeek.Thursday ){
                //     systemDateTime2 = systemDateTime.AddDays( 6 );
                // } else {
                //     systemDateTime2 = systemDateTime.AddDays( -7 + 6 );
                // }
                // if( systemDateTime2.Year == year &&
                //     systemDateTime2.Month == month ){
                //     this.ByMonthDayList.Add( systemDateTime2.Day );
                // }
            }
            this.ByWeeknoEnum = this.Rec.ByWeeknoList.GetEnumerator();
            // this.ByWeeknoListReset = true;
        }

        bool CheckByWeeknoList(){

            if( this.Rec.ByWeeknoList.Count == 0 ){
                return true;
            }

            // BYWEEKNO is not applicable if frequency is not YEARLY
            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;
            if( freq != iCalRecurrence.Frequency.Yearly ){
                return true;
            }

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null ){
                date = datetime.Date;
            }
            if( date == null ){
                return true;
            }
            DateTime systemDateTime = (DateTime)date;

            DateTime from;
            DateTime to;

            foreach( int i in this.Rec.ByWeeknoList ){
                iCalDayOfWeek dow = new iCalDayOfWeek();
                // Thursday is center of week where to count week number
                dow.DayOfWeek = DayOfWeek.Thursday;
                dow.NumOfWeek = i;

                DateTime systemDateTime2;
                DayOfWeek weekStart = this.OriginalRec.WeekStart;

                // previous year
                date = dow.GetFirstDate( systemDateTime.Year - 1 );
                systemDateTime2 = (DateTime)date;
                
                if( weekStart == DayOfWeek.Sunday ){
                    from = systemDateTime2.AddDays( -4 );
                    to = systemDateTime2.AddDays( 2 );
                } else if( weekStart == DayOfWeek.Monday ){
                    from = systemDateTime2.AddDays( -3 );
                    to = systemDateTime2.AddDays( 3 );
                } else if( weekStart == DayOfWeek.Tuesday ){
                    from = systemDateTime2.AddDays( -2 );
                    to = systemDateTime2.AddDays( 4 );
                } else if( weekStart == DayOfWeek.Wednesday ){
                    from = systemDateTime2.AddDays( -1 );
                    to = systemDateTime2.AddDays( 5 );
                } else if( weekStart == DayOfWeek.Thursday ){
                    from = systemDateTime2.AddDays( 0 );
                    to = systemDateTime2.AddDays( 6 );
                } else if( weekStart == DayOfWeek.Friday ){
                    from = systemDateTime2.AddDays( -6 );
                    to = systemDateTime2.AddDays( 0 );
                } else { // if( weekStart == DayOfWeek.Saturday )
                    from = systemDateTime2.AddDays( -5 );
                    to = systemDateTime2.AddDays( 1 );
                }

                if( from <= systemDateTime && systemDateTime <= to ){
                    return true;
                }

                // current year
                date = dow.GetFirstDate( systemDateTime.Year );
                systemDateTime2 = (DateTime)date;
                
                if( weekStart == DayOfWeek.Sunday ){
                    from = systemDateTime2.AddDays( -4 );
                    to = systemDateTime2.AddDays( 2 );
                } else if( weekStart == DayOfWeek.Monday ){
                    from = systemDateTime2.AddDays( -3 );
                    to = systemDateTime2.AddDays( 3 );
                } else if( weekStart == DayOfWeek.Tuesday ){
                    from = systemDateTime2.AddDays( -2 );
                    to = systemDateTime2.AddDays( 4 );
                } else if( weekStart == DayOfWeek.Wednesday ){
                    from = systemDateTime2.AddDays( -1 );
                    to = systemDateTime2.AddDays( 5 );
                } else if( weekStart == DayOfWeek.Thursday ){
                    from = systemDateTime2.AddDays( 0 );
                    to = systemDateTime2.AddDays( 6 );
                } else if( weekStart == DayOfWeek.Friday ){
                    from = systemDateTime2.AddDays( -6 );
                    to = systemDateTime2.AddDays( 0 );
                } else { // if( weekStart == DayOfWeek.Saturday )
                    from = systemDateTime2.AddDays( -5 );
                    to = systemDateTime2.AddDays( 1 );
                }

                if( from <= systemDateTime && systemDateTime <= to ){
                    return true;
                }

                // next year
                date = dow.GetFirstDate( systemDateTime.Year + 1 );
                systemDateTime2 = (DateTime)date;
                
                if( weekStart == DayOfWeek.Sunday ){
                    from = systemDateTime2.AddDays( -4 );
                    to = systemDateTime2.AddDays( 2 );
                } else if( weekStart == DayOfWeek.Monday ){
                    from = systemDateTime2.AddDays( -3 );
                    to = systemDateTime2.AddDays( 3 );
                } else if( weekStart == DayOfWeek.Tuesday ){
                    from = systemDateTime2.AddDays( -2 );
                    to = systemDateTime2.AddDays( 4 );
                } else if( weekStart == DayOfWeek.Wednesday ){
                    from = systemDateTime2.AddDays( -1 );
                    to = systemDateTime2.AddDays( 5 );
                } else if( weekStart == DayOfWeek.Thursday ){
                    from = systemDateTime2.AddDays( 0 );
                    to = systemDateTime2.AddDays( 6 );
                } else if( weekStart == DayOfWeek.Friday ){
                    from = systemDateTime2.AddDays( -6 );
                    to = systemDateTime2.AddDays( 0 );
                } else { // if( weekStart == DayOfWeek.Saturday )
                    from = systemDateTime2.AddDays( -5 );
                    to = systemDateTime2.AddDays( 1 );
                }

                if( from <= systemDateTime && systemDateTime <= to ){
                    return true;
                }
            }

            return false;
        }
        
        void ResetByMonthList(){
            int year = 0;
            // int month = 0;

            iCalDateTime datetime = this.Current as iCalDateTime;
            iCalDate date = this.Current as iCalDate;
            if( datetime != null && datetime.Date != null ){
                year = datetime.Date.Year;
                // month = datetime.Date.Month;
            } else if( date != null ){
                year = date.Year;
                // month = date.Month;
            }
            if( year == 0 ){
                throw new ArgumentException( "not iCalDateTime nor iCalDate" );
            }
            
            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;

            Calendar systemCalendar = new GregorianCalendar();
            int lastMonth = systemCalendar.GetMonthsInYear( year );

            this.Rec.ByMonthList = new SortedSet<int>();
            if( freq == iCalRecurrence.Frequency.Yearly ){
                this.AllMonthList = new SortedSet<int>();
            }
            foreach( int i in this.OriginalRec.ByMonthList ){
                int i2 = i;
                if( i < 0 ){
                    i2 = lastMonth + i + 1;
                }

                this.Rec.ByMonthList.Add( i2 );
                if( freq == iCalRecurrence.Frequency.Yearly ){
                    this.AllMonthList.Add( i2 );
                }
            }

            if( freq == iCalRecurrence.Frequency.Yearly ){
                this.ResetByWeekDayList();
                foreach( iCalDayOfWeek dow in this.Rec.ByWeekDayList ){
                    if( this.Rec.ByMonthList.Count == 0 ){
                        date = dow.GetFirstDate( year );
                        if( dow.NumOfWeek == 0 ){
                            DateTime systemDateTime = (DateTime)date;
                            while( systemDateTime.Year == year ){
                                this.AllMonthList.Add( systemDateTime.Month );
                                systemDateTime = systemDateTime.AddDays( 7 );
                            }
                        } else {
                            if( date.Year == year ){
                                this.AllMonthList.Add( date.Month );
                            }
                        }
                    }
                }

                this.ResetByYearDayList();
                foreach( int i in this.Rec.ByYearDayList ){
                    DateTime systemDateTime = new DateTime( year, 1, 1 );
                    systemDateTime = systemDateTime.AddDays( i - 1 );
                    if( systemDateTime.Year == year ){
                        this.AllMonthList.Add( systemDateTime.Day );
                    }
                }

                this.ResetByWeeknoList();
                foreach( int i in this.Rec.ByWeeknoList ){
                    iCalDayOfWeek dow = new iCalDayOfWeek();
                    dow.NumOfWeek = i;
                    dow.DayOfWeek = DayOfWeek.Thursday;

                    DayOfWeek weekStart = this.OriginalRec.WeekStart;
                    DateTime from, to;

                    // previous year
                    date = dow.GetFirstDate( year -1 );
                    from = (DateTime)date;
                    while( from.DayOfWeek != weekStart ){
                        from = from.AddDays( -1 );
                    }
                    to = from.AddDays( 1 );
                    while( to.DayOfWeek != weekStart ){
                        to = to.AddDays( 1 );
                    }
                    for( DateTime dt = from; dt < to; dt = dt.AddDays( 1 ) ){
                        if( dt.Year == year ){
                            this.AllMonthList.Add( dt.Month );
                        }
                    }

                    // current year
                    date = dow.GetFirstDate( year );
                    from = (DateTime)date;
                    while( from.DayOfWeek != weekStart ){
                        from = from.AddDays( -1 );
                    }
                    to = from.AddDays( 1 );
                    while( to.DayOfWeek != weekStart ){
                        to = to.AddDays( 1 );
                    }
                    for( DateTime dt = from; dt < to; dt = dt.AddDays( 1 ) ){
                        if( dt.Year == year ){
                            this.AllMonthList.Add( dt.Month );
                        }
                    }

                    // next year
                    date = dow.GetFirstDate( year + 1 );
                    from = (DateTime)date;
                    while( from.DayOfWeek != weekStart ){
                        from = from.AddDays( -1 );
                    }
                    to = from.AddDays( 1 );
                    while( to.DayOfWeek != weekStart ){
                        to = to.AddDays( 1 );
                    }
                    for( DateTime dt = from; dt < to; dt = dt.AddDays( 1 ) ){
                        if( dt.Year == year ){
                            this.AllMonthList.Add( dt.Month );
                        }
                    }
                }
            }

            if( freq == iCalRecurrence.Frequency.Yearly ) {
                this.ByMonthEnum = this.AllMonthList.GetEnumerator();
            } else {
                this.ByMonthEnum = this.Rec.ByMonthList.GetEnumerator();
            }
            this.ByMonthListReset = true;
        }

        bool CheckByMonthList(){
            if( this.Rec.ByMonthList.Count == 0 ){
                return true;
            } else {
                iCalDateTime datetime = this.Current as iCalDateTime;
                iCalDate date = this.Current as iCalDate;
                if( datetime != null ){
                    date = datetime.Date;
                }
                if( date == null ){
                    return true;
                } else {
                    int month = date.Month;
                
                    if( this.Rec.ByMonthList.Contains( month ) ) {
                        return true;
                    }
                }
            }
            return false;
        }

        void ResetBySetposList()
        {
            iCalRecurrence.Frequency freq = this.OriginalRec.Freq;
            if( freq == iCalRecurrence.Frequency.Minutely ){
                this.ResetBySetposListMinutely();
            } else if( freq == iCalRecurrence.Frequency.Hourly ){
                this.ResetBySetposListHourly();
            } else if( freq == iCalRecurrence.Frequency.Daily ){
                this.ResetBySetposListDaily();
            } else if( freq == iCalRecurrence.Frequency.Weekly ){
                this.ResetBySetposListWeekly();
            } else if( freq == iCalRecurrence.Frequency.Monthly ){
                this.ResetBySetposListMonthly();
            } else if( freq == iCalRecurrence.Frequency.Yearly ){
                this.ResetBySetposListYearly();
            } else {
                this.ListForSetpos = new SortedSet<iCalTimeRelatedType>();
            }

            this.Rec.BySetposList = new SortedSet<int>();
            foreach( int i in this.OriginalRec.BySetposList ){
                int i2 = i;
                if( i2 < 0 ){
                    i2 = this.ListForSetpos.Count + i2 + 1;
                }
                if( 0 < i2 && i2 <= this.ListForSetpos.Count ){
                    this.Rec.BySetposList.Add( i2 );
                }
            }
            this.BySetposEnum = this.Rec.BySetposList.GetEnumerator();
        }

        iCalTimeRelatedType GetDateTimeBySetPos( int index ){
            IEnumerator<iCalTimeRelatedType> en = this.ListForSetpos.GetEnumerator();
            bool ret = false;
            for( int i = 0; i < index; i++ ){
                ret = en.MoveNext();
            }
            if( ret == true ){
                return en.Current;
            } else {
                return null;
            }
        }

        public void Dispose(){
            this.BySecEnum.Dispose();
            this.ByMinEnum.Dispose();
            this.ByHourEnum.Dispose();
            this.ByWeekDayEnum.Dispose();
            this.ByMonthDayEnum.Dispose();
            this.ByYearDayEnum.Dispose();
            this.ByWeeknoEnum.Dispose();
            this.ByMonthEnum.Dispose();
            this.BySetposEnum.Dispose();
        }
    }
}