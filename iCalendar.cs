// Copyright 2011 Miyako Komooka
using System;
using System.Collections.Generic;
using System.IO;


namespace iCalLibrary // based on rfc5545
{
    using DataType;
    using Property;
    using Component;

    public class iCalendar : iCalComponent
    {
        public static String RepName = "vcalendar";

        // property of iCalender itself
        public iCalProductIdentifier ProductId;
        public iCalVersion Version;
        public iCalScale Scale;
        public iCalMethod Method;

        public Dictionary<String, iCalTimeZone> TimeZones =
            new Dictionary<String, iCalTimeZone>();

        public Dictionary<String, iCalTimeZone> DefaultTimeZones = null;

        public List<iCalEvent> EventList = new List<iCalEvent>();
        public List<iCalToDo>  ToDoList  = new List<iCalToDo>();
        public List<iCalJournal> JournalList = new List<iCalJournal>();
        public List<iCalFreeBusy> FreeBusyList = new List<iCalFreeBusy>();

        // list of components not defined in rfc ( x-comp / iana-comp )
        public List<iCalComponent> OtherList = new List<iCalComponent>();
        
        public override void AddChild( iCalComponent child ){
            if( child is iCalEvent ){
                this.EventList.Add( (iCalEvent)child );
            } else if( child is iCalToDo ){
                this.ToDoList.Add( (iCalToDo)child );
            } else if( child is iCalJournal ){
                this.JournalList.Add( (iCalJournal)child );
            } else if( child is iCalFreeBusy ){
                this.FreeBusyList.Add( (iCalFreeBusy) child );
            } else if( child is iCalTimeZone ){
                iCalTimeZone timezone = (iCalTimeZone)child;
                String id = timezone.TimeZoneId.Value;
                this.TimeZones[ id ] = timezone;
            } else {
                this.OtherList.Add( child );
            }
        }

        public override void SetProductIdentifier( iCalLineContent content ){
            this.ProductId = new iCalProductIdentifier( content, this );
        }

        public override void SetVersion( iCalLineContent content ){
            this.Version = new iCalVersion( content, this );
        }

        public override void SetScale( iCalLineContent content ){
            this.Scale = new iCalScale( content, this );
        }

        public override void SetMethod( iCalLineContent content ){
            this.Method = new iCalMethod( content, this );
        }

        public List<iCalEvent> GetEventByDay( int year, int month, int day ) {
            return GetEventByDay( year, month, day, TimeZoneInfo.Local );
        }

        public List<iCalEvent> GetEventByDay( int year, int month, int day,
                                              TimeZoneInfo tzInfo )
        {
            List<iCalEvent> ret = new List<iCalEvent>();
            Dictionary<string,bool> uidDic = new Dictionary<string,bool>();


            foreach( iCalEvent ev in this.EventList ){
                List<iCalEvent> event1List = ev.GetEventByDay( year, month, day, tzInfo );
                
                if( ev.UID != null && event1List.Count > 0 ){
                    string uid = ev.UID.Value.Text;
                    if( uidDic.ContainsKey( uid ) ){
                        while( true ){
                            loop:
                            foreach( iCalEvent oldEv in ret ){
                                if( oldEv.UID == null ||
                                    oldEv.UID.Value.Text != uid ){
                                    continue;
                                }

                                // for( int i = 0; i < event1List.Count; i++ ){
                                foreach( iCalEvent newEv in event1List ){
                                    // iCalEvent newEv = event1List[i];
                                    if( newEv.RecurrenceId == oldEv.DateTimeStart ){
                                        if( newEv.SequenceNum > oldEv.SequenceNum ){
                                            ret.Remove( oldEv );
                                            ret.Add( newEv );
                                            // event1List.Remove( newEv );
                                            goto loop;
                                        } else {
                                            // event1List.Remove( newEv );
                                            // break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    } else {
                        ret.AddRange( event1List );
                        uidDic[ uid ] = true;
                    }
                } else {
                    ret.AddRange( event1List );
                }
            }

            return ret;
        }

        public List<iCalToDo> GetToDoByDay( int year, int month, int day,
                                            bool includesNoDateEntry )
        {
            return GetToDoByDay( year, month, day, includesNoDateEntry,
                                 TimeZoneInfo.Local );
        }

        public List<iCalToDo> GetToDoByDay( int year, int month, int day,
                                            bool includesNoDateEntry,
                                            TimeZoneInfo tzInfo )
        {
            List<iCalToDo> ret = new List<iCalToDo>();

            foreach( iCalToDo ev in this.ToDoList ){
                ret.AddRange( ev.GetToDoByDay( year, month, day,
                                               includesNoDateEntry,  tzInfo ) );
            }

            return ret;
        }

        public void SetDefaultTimeZones()
        {
            String str = this.GetDefaultTimeZoneString();
            StringReader reader = new StringReader( str );

            iCalParser parser = new iCalParser();
            iCalendarCollection coll = parser.ParseStream( reader );

            iCalendar cal = coll.CalendarList[0];
            this.DefaultTimeZones = cal.TimeZones;
        }

        public override iCalTimeZone GetTimeZoneById( String timeZoneId ){
            if( timeZoneId != null ){
                String id = timeZoneId;

                if( this.TimeZones.ContainsKey( id ) ){
                    return this.TimeZones[ id ];
                } else {
                    if( this.DefaultTimeZones == null ){
                        this.SetDefaultTimeZones();
                    }

                    if( this.DefaultTimeZones.ContainsKey( id ) ){
                        return this.DefaultTimeZones[ id ];
                    }
                }
            }

            return null;
        }

        String GetDefaultTimeZoneString() {
            String ret =
                "begin:vcalendar\n" +
                "BEGIN:VTIMEZONE\n" +
                "TZID:Asia/Tokyo\n" +
                "BEGIN:STANDARD\n" +
                "TZOFFSETFROM:+0900\n" +
                "TZOFFSETTO:+0900\n" +
                "TZNAME:JST\n" +
                "DTSTART:19700101T000000\n" +
                "END:STANDARD\n" +
                "END:VTIMEZONE\n" +
                "BEGIN:VTIMEZONE\n" +
                "TZID:US/Eastern\n" +
                "BEGIN:DAYLIGHT\n" +
                "TZOFFSETFROM:-0500\n" +
                "TZOFFSETTO:-0400\n" +
                "DTSTART:20070311T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU\n" +
                "TZNAME:EDT\n" +
                "END:DAYLIGHT\n" +
                "BEGIN:STANDARD\n" +
                "TZOFFSETFROM:-0400\n" +
                "TZOFFSETTO:-0500\n" +
                "DTSTART:20071104T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU\n" +
                "TZNAME:EST\n" +
                "END:STANDARD\n" +
                "END:VTIMEZONE\n" +
                "BEGIN:VTIMEZONE\n" +
                "TZID:US/Pacific\n" +
                "BEGIN:DAYLIGHT\n" +
                "TZOFFSETFROM:-0800\n" +
                "TZOFFSETTO:-0700\n" +
                "DTSTART:20070311T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU\n" +
                "TZNAME:PDT\n" +
                "END:DAYLIGHT\n" +
                "BEGIN:STANDARD\n" +
                "TZOFFSETFROM:-0700\n" +
                "TZOFFSETTO:-0800\n" +
                "DTSTART:20071104T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU\n" +
                "TZNAME:PST\n" +
                "END:STANDARD\n" +
                "END:VTIMEZONE\n" +
                "begin:vtimezone\n" +
                "tzid:US/Central\n" +
                "begin:daylight\n" +
                "DTSTART:20070311T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU\n" +
                "TZOFFSETTO:-0500\n" +
                "end:daylight\n" +
                "begin:standard\n" +
                "DTSTART:20071104T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU\n" +
                "TZOFFSETTO:-0600\n" +
                "end:standard\n" +
                "end:vtimezone\n" +
                "begin:vtimezone\n" +
                "tzid:US/Mountain\n" +
                "begin:daylight\n" +
                "DTSTART:20070311T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU\n" +
                "TZOFFSETTO:-0600\n" +
                "end:daylight\n" +
                "begin:standard\n" +
                "DTSTART:20071104T020000\n" +
                "RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU\n" +
                "TZOFFSETTO:-0700\n" +
                "end:standard\n" +
                "end:vtimezone\n" +
                "end:vcalendar\n";

            return ret;
        }
    }

    public class iCalendarCollection : iCalComponent
    {
        public List<iCalendar> CalendarList = new List<iCalendar>();

        public override void AddChild( iCalComponent child ){
            iCalendar calendar = child as iCalendar;
            if( calendar != null ){
                this.CalendarList.Add( calendar );
            }
        }

        public List<iCalEvent> GetEventByDay( int year, int month, int day )
        {
            List<iCalEvent> ret = new List<iCalEvent>();


            foreach( iCalendar cal in this.CalendarList ){
                ret.AddRange( cal.GetEventByDay( year, month, day ) );
            }

            return ret;
        }

        public List<iCalEvent> GetEventByDay( int year, int month, int day,
                                              TimeZoneInfo tzInfo )
        {
            List<iCalEvent> ret = new List<iCalEvent>();


            foreach( iCalendar cal in this.CalendarList ){
                ret.AddRange( cal.GetEventByDay( year, month, day,tzInfo ) );
            }

            return ret;
        }

        public List<iCalToDo> GetToDoByDay( int year, int month, int day,
                                            bool includesNoDateEntry )
        {
            List<iCalToDo> ret = new List<iCalToDo>();


            foreach( iCalendar cal in this.CalendarList ){
                ret.AddRange( cal.GetToDoByDay( year, month, day,
                                                includesNoDateEntry ) );
            }

            return ret;
        }

        public List<iCalToDo> GetToDoByDay( int year, int month, int day,
                                            bool includesNoDateEntry,
                                            TimeZoneInfo tzInfo )
        {
            List<iCalToDo> ret = new List<iCalToDo>();


            foreach( iCalendar cal in this.CalendarList ){
                ret.AddRange( cal.GetToDoByDay( year, month, day,
                                                includesNoDateEntry, tzInfo ) );
            }

            return ret;
        }

    }
}