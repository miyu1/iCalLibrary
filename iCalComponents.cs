// Copyright 2011 Miyako Komooka
using System;
using System.Collections.Generic;
using System.Collections;

// Calendar Components Defined in rfc5545 section 3.6
namespace iCalLibrary.Component
{
    using Property;
    using DataType;

    // icalstream = 1*icalobject
    // icalobject = "BEGIN" ":" "VCALENDAR" CRLF
    //              icalbody
    //              "END" ":" "VCALENDAR" CRLF
    // icalbody   = calprops component
    // calprops   = *(
    //            ;
    //            ; The following are REQUIRED,
    //            ; but MUST NOT occur more than once.
    //            ;
    //            prodid / version /
    //            ;
    //            ; The following are OPTIONAL,
    //            ; but MUST NOT occur more than once.
    //            ;
    //            calscale / method /
    //            ;
    //            ; The following are OPTIONAL,
    //            ; and MAY occur more than once.
    //            ;
    //            x-prop / iana-prop
    //            ;
    //            )
    // component  = 1*(eventc / todoc / journalc / freebusyc /
    //              timezonec / iana-comp / x-comp)
    // iana-comp  = "BEGIN" ":" iana-token CRLF
    //              1*contentline
    //              "END" ":" iana-token CRLF
    // x-comp     = "BEGIN" ":" x-name CRLF
    //              1*contentline
    //              "END" ":" x-name CRLF


    // Property List:
    //  R: Required, O: Optional, M: Multiple Value
    //
    //                Event ToDo Journal F/B TimeZone TZElem Alarm
    // dtstart        O     O    O       O   -        R      -
    // comment        M     M    M       M   -        M      -
    // attendee       M     M    M       M   -        -      M
    // dtstamp        R     R    R       R   -        -      -
    // uid            R     R    R       R   -        -      -
    // rstatus        M     M    M       M   -        -      -
    // url            O     O    O       O   -        -      -
    // organizer      O     O    O       O   -        -      -
    // contact        M     M    M       O   -        -      -
    // last-mod       O     O    O       -   O        -      -
    // rrule          O     O    O       -   -        O      -
    // rdate          M     M    M       -   -        M      -
    // attach         M     M    M       -   -        -      M
    // description    O     O    M       -   -        -      O
    // categories     M     M    M       -   -        -      -
    // class          O     O    O       -   -        -      -
    // created        O     O    O       -   -        -      -
    // exdate         M     M    M       -   -        -      -
    // recurid        O     O    O       -   -        -      -
    // related        M     M    M       -   -        -      -
    // seq            O     O    O       -   -        -      -
    // status         O     O    O       -   -        -      -
    // summary        O     O    O       -   -        -      O
    // duration       O     O    -       -   -        -      O
    // resources      M     M    -       -   -        -      -
    // geo            O     O    -       -   -        -      -
    // priority       O     O    -       -   -        -      -
    // location       O     O    -       -   -        -      -
    // dtend          O     -    -       O   -        -      -
    // transp         O     -    -       -   -        -      -
    // completed      -     O    -       -   -        -      -
    // percent        -     O    -       -   -        -      -
    // due            -     O    -       -   -        -      -
    // freebusy       -     -    -       M   -        -      -
    // tzid           -     -    -       -   R        -      -
    // tzurl          -     -    -       -   O        -      -
    // tzname         -     -    -       -   -        M      -
    // tzoffsetto     -     -    -       -   -        O      -
    // tzoffsetfrom   -     -    -       -   -        O      -
    // action         -     -    -       -   -        -      R
    // repeat         -     -    -       -   -        -      O
    // trigger        -     -    -       -   -        -      R


    public class iCalComponent /* : ICloneable */ {
        public iCalComponent Parent = null;

        public Dictionary<String, List<iCalProperty>> Properties =
            new Dictionary<String, List<iCalProperty>>();


        public virtual void AddChild( iCalComponent child ){
            // by defaut do nothing
        }


        public void SetProperty( iCalLineContent content ){            

            if( content.Name == iCalScale.RepName ){
                this.SetScale( content );
            } else if( content.Name == iCalMethod.RepName ){
                this.SetMethod( content );
            } else if( content.Name == iCalProductIdentifier.RepName ){
                this.SetProductIdentifier( content );
            } else if( content.Name == iCalVersion.RepName ){
                this.SetVersion( content );
            } else if( content.Name == iCalAttachment.RepName ){
                this.SetAttachment( content );
            } else if( content.Name == iCalCategories.RepName ){
                this.SetCategories( content );
            } else if( content.Name == iCalClassification.RepName ){
                this.SetClassification( content );
            } else if( content.Name == iCalComment.RepName ){
                this.SetComment( content );
            } else if( content.Name == iCalDescription.RepName ){
                this.SetDescription( content );
            } else if( content.Name == iCalGeographicPosition.RepName ){
                this.SetGeographicPosition( content );
            } else if( content.Name == iCalLocation.RepName ){
                this.SetLocation( content );
            } else if( content.Name == iCalPercentComplete.RepName ){
                this.SetPercentComplete( content );
            } else if( content.Name == iCalPriority.RepName ){
                this.SetPriority( content );
            } else if( content.Name == iCalResources.RepName ){
                this.SetResources( content );
            } else if( content.Name == iCalStatus.RepName ){
                this.SetStatus( content );
            } else if( content.Name == iCalSummary.RepName ){
                this.SetSummary( content );
            } else if( content.Name == iCalDateTimeCompleted.RepName ){
                this.SetDateTimeCompleted( content );
            } else if( content.Name == iCalDateTimeEnd.RepName ){
                this.SetDateTimeEnd( content );
            } else if( content.Name == iCalDateTimeDue.RepName ){
                this.SetDateTimeDue( content );
            } else if( content.Name == iCalDateTimeStart.RepName ){
                this.SetDateTimeStart( content );
            } else if( content.Name == iCalDurationProperty.RepName ){
                this.SetDurationProperty( content );
            } else if( content.Name == iCalFreeBusyTime.RepName ){
                this.SetFreeBusyTime( content );
            } else if( content.Name == iCalTransparency.RepName ){
                this.SetTransparency( content );
            } else if( content.Name == iCalTimeZoneIdentifierProperty.RepName ){
                this.SetTimeZoneIdentifierProperty( content );
            } else if( content.Name == iCalTimeZoneName.RepName ){
                this.SetTimeZoneName( content );
            } else if( content.Name == iCalTimeZoneOffsetFrom.RepName ){
                this.SetTimeZoneOffsetFrom( content );
            } else if( content.Name == iCalTimeZoneOffsetTo.RepName ){
                this.SetTimeZoneOffsetTo( content );
            } else if( content.Name == iCalTimeZoneURL.RepName ){
                this.SetTimeZoneURL( content );
            } else if( content.Name == iCalAttendee.RepName ){
                this.SetAttendee( content );
            } else if( content.Name == iCalContact.RepName ){
                this.SetContact( content );
            } else if( content.Name == iCalOrganizer.RepName ){
                this.SetOrganizer( content );
            } else if( content.Name == iCalRecurrenceId.RepName ){
                this.SetRecurrenceId( content );
            } else if( content.Name == iCalRelatedTo.RepName ){
                this.SetRelatedTo( content );
            } else if( content.Name == iCalURL.RepName ){
                this.SetURL( content );
            } else if( content.Name == iCalUID.RepName ){
                this.SetUID( content );
            } else if( content.Name == iCalExceptionDateTimes.RepName ){
                this.SetExceptionDateTimes( content );
            } else if( content.Name == iCalRecurrenceDateTimes.RepName ){
                this.SetRecurrenceDateTimes( content );
            } else if( content.Name == iCalRecurrenceRule.RepName ){
                this.SetRecurrenceRule( content );
            } else if( content.Name == iCalAction.RepName ){
                this.SetAction( content );
            } else if( content.Name == iCalRepeatCount.RepName ){
                this.SetRepeatCount( content );
            } else if( content.Name == iCalTrigger.RepName ){
                this.SetTrigger( content );
            } else if( content.Name == iCalDateTimeCreated.RepName ){
                this.SetDateTimeCreated( content );
            } else if( content.Name == iCalDateTimeStamp.RepName ){
                this.SetDateTimeStamp( content );
            } else if( content.Name == iCalLastModified.RepName ){
                this.SetLastModified( content );
            } else if( content.Name == iCalSequenceNumber.RepName ){
                this.SetSequenceNumber( content );
            } else if( content.Name == iCalRequestStatus.RepName ){
                this.SetRequestStatus( content );
            } else {
                this.SetGeneralProperty( content );
            }

        }

        public void SetGeneralProperty( iCalLineContent content ){
            List<iCalProperty> list;

            if( this.Properties.ContainsKey( content.Name ) ){
                list = this.Properties[ content.Name ];
            } else {
                list = new List<iCalProperty>();
            }
                   
            list.Add( new iCalProperty( content, this ) );
            this.Properties[ content.Name ] = list;
        }

        public virtual void SetScale( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetMethod( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetProductIdentifier( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetVersion( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetAttachment( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetCategories( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetClassification( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetComment( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDescription( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetGeographicPosition( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetLocation( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetPercentComplete( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetPriority( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetResources( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetStatus( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetSummary( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDateTimeCompleted( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDateTimeEnd( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDateTimeDue( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDateTimeStart( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDurationProperty( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetFreeBusyTime( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTransparency( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTimeZoneIdentifierProperty( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTimeZoneName( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTimeZoneOffsetFrom( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTimeZoneOffsetTo( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTimeZoneURL( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetAttendee( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetContact( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetOrganizer( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetRecurrenceId( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetRelatedTo( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetURL( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetUID( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetExceptionDateTimes( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetRecurrenceDateTimes( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetRecurrenceRule( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetAction( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetRepeatCount( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetTrigger( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDateTimeCreated( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetDateTimeStamp( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetLastModified( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetSequenceNumber( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual void SetRequestStatus( iCalLineContent content ){
            this.SetGeneralProperty( content );
        }

        public virtual Object Clone()
        {
            iCalComponent ret = (iCalComponent)this.MemberwiseClone();

            ret.Properties = new Dictionary<String, List<iCalProperty>>();
            foreach( String key in this.Properties.Keys ){
                List<iCalProperty> originalList = this.Properties[ key ];
                List<iCalProperty> list = new List<iCalProperty>();

                foreach( iCalProperty prop in originalList ){
                    list.Add( (iCalProperty)prop.Clone() ); 
                }

                ret.Properties[ key ] = list;
            }

            return ret;
        }

        public virtual iCalTimeZone GetTimeZoneById( String TimeZoneId ){
            if( this.Parent != null ){
                return this.Parent.GetTimeZoneById( TimeZoneId );
            }
            return null;
        }
    }

    // base class for Event, Todo and Journal
    public abstract class iCalEntry : iCalComponent
    {
        public iCalDateTimeStamp DateTimeStamp;
        public iCalUID UID;
        public iCalDateTimeStart DateTimeStart;
        public iCalRecurrenceRule RRule;
        public iCalRecurrenceId RecurrenceId;
        public iCalURL URL;
        public iCalDescription Description;
        public iCalOrganizer Organizer;
        public iCalLastModified LastModified;
        public iCalClassification Classification;
        public iCalDateTimeCreated DateTimeCreated;
        public iCalSequenceNumber SequenceNum;
        public iCalStatus Status;
        public iCalSummary Summary;

        public List<iCalComment> CommentList = new List<iCalComment>();
        public List<iCalAttendee> AttendeeList = new List<iCalAttendee>();
        public List<iCalRequestStatus> RequestStatusList =
            new List<iCalRequestStatus>();
        public List<iCalContact> ContactList = new List<iCalContact>();
        public List<iCalRecurrenceDateTimes> RecurrenceDateList =
            new List<iCalRecurrenceDateTimes>();
        public List<iCalAttachment> AttachmentList =
            new List<iCalAttachment>();
        public List<iCalCategories> CategoriesList =
            new List<iCalCategories>();
        public List<iCalExceptionDateTimes> ExceptionDateList =
            new List<iCalExceptionDateTimes>();
        public List<iCalRelatedTo> RelatedToList =
            new List<iCalRelatedTo>();


        public override void SetDateTimeStamp( iCalLineContent content ){
            this.DateTimeStamp = new iCalDateTimeStamp( content, this );
        }

        public override void SetUID( iCalLineContent content ){
            this.UID = new iCalUID( content, this );
        }
        
        public override void SetDateTimeStart( iCalLineContent content ){
            this.DateTimeStart = new iCalDateTimeStart( content, this );
        }
            
        public override void SetRecurrenceRule( iCalLineContent content ){
            this.RRule = new iCalRecurrenceRule( content, this );
        }
        
        public override void SetRecurrenceId( iCalLineContent content ){
            this.RecurrenceId = new iCalRecurrenceId( content, this );
        }
        
        public override void SetURL( iCalLineContent content ){
            this.URL = new iCalURL( content, this );
        }
        
        public override void SetDescription( iCalLineContent content ){
            this.Description = new iCalDescription( content, this );
        }
        
        public override void SetOrganizer( iCalLineContent content ){
            this.Organizer = new iCalOrganizer( content, this );
        }
        
        public override void SetLastModified( iCalLineContent content ){
            this.LastModified = new iCalLastModified( content, this );
        }
        
        public override void SetClassification( iCalLineContent content ){
            this.Classification = new iCalClassification( content, this );
        }
        
        public override void SetDateTimeCreated( iCalLineContent content ){
            this.DateTimeCreated = new iCalDateTimeCreated( content, this );
        }
        
        public override void SetSequenceNumber( iCalLineContent content ){
            this.SequenceNum = new iCalSequenceNumber( content, this );
        }
        
        public override void SetStatus( iCalLineContent content ){
            this.Status = new iCalStatus( content, this );
        }
        
        public override void SetSummary( iCalLineContent content ){
            this.Summary = new iCalSummary( content, this );
        }
        
        public override void SetComment( iCalLineContent content ){
            this.CommentList.Add( new iCalComment( content, this ) );
        }
        
        public override void SetAttendee( iCalLineContent content ){
            this.AttendeeList.Add( new iCalAttendee( content, this ) );
        }
        
        public override void SetRequestStatus( iCalLineContent content ){
            this.RequestStatusList.Add( new iCalRequestStatus( content, this ) );
        }
        
        public override void SetContact( iCalLineContent content ){
            this.ContactList.Add( new iCalContact( content, this ) );
        }
        
        public override void SetRecurrenceDateTimes( iCalLineContent content ){
            this.RecurrenceDateList.Add( new iCalRecurrenceDateTimes( content, this ) );
        }
        
        public override void SetAttachment( iCalLineContent content ){
            this.AttachmentList.Add( new iCalAttachment( content, this ) );
        }
        
        public override void SetCategories( iCalLineContent content ){
            this.CategoriesList.Add( new iCalCategories( content, this ) );
        }
        
        public override void SetExceptionDateTimes( iCalLineContent content ){
            this.ExceptionDateList.Add( new iCalExceptionDateTimes( content, this ) );
        }
        
        public override void SetRelatedTo( iCalLineContent content ){
            this.RelatedToList.Add( new iCalRelatedTo( content, this ) );
        }


        public bool IsExceptionDate()
        {
            return this.IsExceptionDate( this.DateTimeStart );
        }

        public bool IsExceptionDate( iCalDateTimeStart dateTimeStart ){

            // iCalDateTime datetime = timeRelated as iCalDateTime;
            // iCalDate date = timeRelated as iCalDate;

            if( this.ExceptionDateList == null ){
                return false;
            }

            // bool timeZoneMatch = false;
            // iCalTimeRelatedType starttime = dateTimeStart.DateTime;
            // iCalDateTimeStart dateTimeStart = this.DateTimeStart;
            iCalDateTimeStart dateTimeStart2 = dateTimeStart;

            foreach( iCalExceptionDateTimes exdates in this.ExceptionDateList ){
                iCalExceptionDateTimes exdates1 = exdates;
                if( exdates.TimeZoneId != dateTimeStart.TimeZoneId ){
                    exdates1 = exdates.ToUTC();
                    dateTimeStart2
                        = (iCalDateTimeStart)dateTimeStart.ToUTC();
                }

                foreach( iCalTimeRelatedType timeRelated in exdates1.Values ){
                    iCalDate date = timeRelated as iCalDate;

                    if( date != null ){
                        if( date == dateTimeStart.Value ){
                            return true;
                        }
                    } else if( timeRelated == dateTimeStart2.Value ){
                        return true;
                    }
                }
            }


            return false;
        }

        public override Object Clone()
        {
            iCalEntry ret = (iCalEntry)base.Clone();

            if( this.DateTimeStamp != null ){
                ret.DateTimeStamp =
                    (iCalDateTimeStamp)this.DateTimeStamp.Clone();
            }

            if( this.UID != null ){
                ret.UID = (iCalUID)this.UID.Clone();
            }
            
            if( this.DateTimeStart != null ){
                ret.DateTimeStart =
                    (iCalDateTimeStart)this.DateTimeStart.Clone();
            }
            
            if( this.RRule != null ){
                ret.RRule = (iCalRecurrenceRule)this.RRule.Clone();
            }

            if( this.RecurrenceId != null ){
                ret.RecurrenceId = (iCalRecurrenceId)this.RecurrenceId.Clone();
            }
            
            if( this.URL != null ){
                ret.URL = (iCalURL)this.URL.Clone();
            }
            
            if( this.Description != null ){
                ret.Description = (iCalDescription)this.Description.Clone();
            }

            if( this.Organizer != null ){
                ret.Organizer = (iCalOrganizer)this.Organizer.Clone();
            }
            
            if( this.LastModified != null ){
                ret.LastModified = (iCalLastModified)this.LastModified.Clone();
            }
            
            if( this.Classification != null ){
                ret.Classification =
                    (iCalClassification)this.Classification.Clone();
            }

            if( this.DateTimeCreated != null ){
                ret.DateTimeCreated = (iCalDateTimeCreated)this.DateTimeCreated.Clone();
            }

            if( this.SequenceNum != null ){
                ret.SequenceNum = (iCalSequenceNumber)this.SequenceNum.Clone();
            }

            if( this.Status != null ){
                ret.Status = (iCalStatus)this.Status.Clone();
            }
            
            if( this.Summary != null ){
                ret.Summary = (iCalSummary)this.Summary.Clone();
            }
            

            ret.CommentList = new List<iCalComment>();
            foreach( iCalComment comment in this.CommentList ){
                ret.CommentList.Add( (iCalComment)comment.Clone() );
            }
            
            ret.AttendeeList = new List<iCalAttendee>();
            foreach( iCalAttendee elem in this.AttendeeList ){
                ret.AttendeeList.Add( (iCalAttendee)elem.Clone() );
            }

            ret.RequestStatusList = new List<iCalRequestStatus>();
            foreach( iCalRequestStatus elem in this.RequestStatusList ){
                ret.RequestStatusList.Add( (iCalRequestStatus)elem.Clone() );
            }

            ret.ContactList = new List<iCalContact>();
            foreach( iCalContact elem in this.ContactList ){
                ret.ContactList.Add( (iCalContact)elem.Clone() );
            }

            ret.RecurrenceDateList = new List<iCalRecurrenceDateTimes>();
            foreach( iCalRecurrenceDateTimes elem in this.RecurrenceDateList ){
                ret.RecurrenceDateList.Add( (iCalRecurrenceDateTimes)elem.Clone() );
            }

            ret.AttachmentList = new List<iCalAttachment>();
            foreach( iCalAttachment elem in this.AttachmentList ){
                ret.AttachmentList.Add( (iCalAttachment)elem.Clone() );
            }

            ret.CategoriesList = new List<iCalCategories>();
            foreach( iCalCategories elem in this.CategoriesList ){
                ret.CategoriesList.Add( (iCalCategories)elem.Clone() );
            }

            ret.ExceptionDateList = new List<iCalExceptionDateTimes>();
            foreach( iCalExceptionDateTimes elem in this.ExceptionDateList ){
                ret.ExceptionDateList.Add((iCalExceptionDateTimes)elem.Clone());
            }

            ret.RelatedToList = new List<iCalRelatedTo>();
            foreach( iCalRelatedTo elem in this.RelatedToList ){
                ret.RelatedToList.Add((iCalRelatedTo)elem.Clone());
            }

            return ret;
        }

        public SortedSet<iCalDateTimeStart> GetSortedRecurrenceDateTimes()
        {
            SortedSet<iCalDateTimeStart> ret =
                new SortedSet<iCalDateTimeStart>( /* (IComparer<iCalDateTimeStart>)this */ );

            foreach( iCalRecurrenceDateTimes rdates in this.RecurrenceDateList )
            {
                foreach( iCalTimeRelatedType timeRelated in rdates.Values ){

                    iCalDateTimeStart dtstart = new iCalDateTimeStart( this );

                    dtstart.Value = (iCalTimeRelatedType)timeRelated.Clone();
                    dtstart.TimeZoneId = rdates.TimeZoneId;

                    ret.Add( dtstart );

                }
            }
            
            return ret;
        }
    }

    public class iCalEvent
        : iCalEntry, IEnumerable<iCalEvent>
    {
        // eventc     = "BEGIN" ":" "VEVENT" CRLF
        //              eventprop *alarmc
        //              "END" ":" "VEVENT" CRLF
        // eventprop  = *(
        //            ;
        //            ; The following are REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            dtstamp / uid /
        //            ;
        //            ; The following is REQUIRED if the component
        //            ; appears in an iCalendar object that doesn't
        //            ; specify the "METHOD" property; otherwise, it
        //            ; is OPTIONAL; in any case, it MUST NOT occur
        //            ; more than once.
        //            ;
        //            dtstart /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            class / created / description / geo /
        //            last-mod / location / organizer / priority /
        //            seq / status / summary / transp /
        //            url / recurid /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but SHOULD NOT occur more than once.
        //            ;
        //            rrule /
        //            ;
        //            ; Either 'dtend' or 'duration' MAY appear in
        //            ; a 'eventprop', but 'dtend' and 'duration'
        //            ; MUST NOT occur in the same 'eventprop'.
        //            ;
        //            dtend / duration /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            attach / attendee / categories / comment /
        //            contact / exdate / rstatus / related /
        //            resources / rdate / x-prop / iana-prop
        //            ;
        //            )
        public static String RepName = "vevent";

        // original event if rrule exists or datetime conversion needed
        public iCalEvent Original = null;

        private iCalDurationProperty duration;
        public iCalDurationProperty Duration {
            set { this.duration = value; }
            get {
                if( this.duration != null ){
                    return this.duration ;
                } else if( this.DateTimeStart != null &&
                           this.dateTimeEnd != null ){
                    iCalDurationProperty duration =
                        this.dateTimeEnd - this.DateTimeStart;
                    return duration;
                }
                return null;
            }
        }

        public iCalGeographicPosition Geo;
        public iCalPriority Priority;
        public iCalLocation Location;
        private iCalDateTimeEnd dateTimeEnd;
        public iCalDateTimeEnd DateTimeEnd {
            set { this.dateTimeEnd = value; }
            get {
                if( this.dateTimeEnd != null ){
                    return this.dateTimeEnd;
                } else if( this.DateTimeStart != null &&
                           this.duration != null ){

                    iCalDateTimeEnd datetimeend =
                        new iCalDateTimeEnd( this.DateTimeStart, this );

                    datetimeend.Value += this.Duration.Value;
                    return datetimeend;
                }
                return null;
            }
        }
        public iCalTransparency Transparency;
        
        public List<iCalResources> ResourceList =
            new List<iCalResources>();

        public List<iCalAlarm> AlarmList = new List<iCalAlarm>();


        public override Object Clone()
        {
            iCalEvent ret = (iCalEvent)base.Clone();

            ret.Original = this.Original;
            if( this.duration != null ){
                ret.duration = (iCalDurationProperty)this.duration.Clone();
            }
            if( this.Geo != null ){
                ret.Geo = (iCalGeographicPosition)this.Geo.Clone();
            }
            if( this.Priority != null ){
                ret.Priority = (iCalPriority)this.Priority.Clone();
            }
            if( this.Location != null ){
                ret.Location = (iCalLocation)this.Location.Clone();
            }
            if( this.dateTimeEnd != null ){
                ret.dateTimeEnd = (iCalDateTimeEnd)this.dateTimeEnd.Clone();
            }
            if( this.Transparency != null ){
                ret.Transparency = (iCalTransparency)this.Transparency.Clone();
            }

            ret.ResourceList = new List<iCalResources>();
            foreach( iCalResources elem in this.ResourceList ){
                ret.ResourceList.Add( (iCalResources)elem.Clone() );
            }

            ret.AlarmList = new List<iCalAlarm>();
            foreach( iCalAlarm elem in this.AlarmList ){
                ret.AlarmList.Add( (iCalAlarm)elem.Clone() );
            }

            return ret;
        }

        public override void AddChild( iCalComponent child ){
            if( child is iCalAlarm ){
                this.AlarmList.Add( (iCalAlarm)child );
            }
        }

        public override void SetDurationProperty( iCalLineContent content ){
            this.Duration = new iCalDurationProperty( content, this );
        }

        public override void SetGeographicPosition( iCalLineContent content ){
            this.Geo = new iCalGeographicPosition( content, this );
        }

        public override void SetPriority( iCalLineContent content ){
            this.Priority = new iCalPriority( content, this );
        }

        public override void SetLocation( iCalLineContent content ){
            this.Location = new iCalLocation( content, this );
        }

        public override void SetDateTimeEnd( iCalLineContent content ){
            this.DateTimeEnd = new iCalDateTimeEnd( content, this );
        }

        public override void SetTransparency( iCalLineContent content ){
            this.Transparency = new iCalTransparency( content, this );
        }

        public override void SetResources( iCalLineContent content ){
            this.ResourceList.Add( new iCalResources( content, this ) );
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }

        IEnumerator<iCalEvent> IEnumerable<iCalEvent>.GetEnumerator()
        {
            return (IEnumerator<iCalEvent>) GetEnumerator();
        }

        public iCalEventEnum GetEnumerator()
        {
            return new iCalEventEnum( this );
        }

        public iCalEvent
            RecalcFromStartTime( iCalDateTimeStart dateTimeStart )
                                 //iCalTimeRelatedType timeRelated,
                                 // String timeZoneId )
        {
            iCalEvent ret = (iCalEvent)this.Clone();
            ret.Original = this;
            
            if( dateTimeStart.Value is iCalDate &&
                ret.DateTimeStart.Value is iCalDateTime ) {

                ((iCalDateTime)ret.DateTimeStart.Value).Date =
                    (iCalDate)dateTimeStart.Value;

            } else {
                ret.DateTimeStart.Value = dateTimeStart.Value;
                ret.DateTimeStart.TimeZoneId = dateTimeStart.TimeZoneId;
            }

            // ret.DateTimeStart.Component = ret; // how about other properties?

            if( ret.duration != null ){
                // already copied by Clone(). 
                ret.dateTimeEnd = null;
            } else if( this.dateTimeEnd != null ){
                iCalDurationProperty duration = this.Duration;

                ret.dateTimeEnd =
                    new iCalDateTimeEnd( ret.DateTimeStart + duration,
                                         ret);

            }
            return ret;
        }

        public bool OnDay( iCalDate date )
        {
            iCalTime time = new iCalTime();

            iCalDateTime dateTimeStart = new iCalDateTime();
            dateTimeStart.Date = date;
            dateTimeStart.Time = time;


            iCalDateTime dateTimeEnd = new iCalDateTime();
            dateTimeEnd.Date = date.NextDay();
            dateTimeEnd.Time = time;

            iCalDateTime evStart = this.DateTimeStart.Value as iCalDateTime;
            iCalDateTime evEnd = this.DateTimeEnd.Value as iCalDateTime;

            iCalDate dStart = this.DateTimeStart.Value as iCalDate;
            if( dStart != null ){
                evStart = new iCalDateTime();
                evStart.Date = dStart;
                evStart.Time = time;
            }

            iCalDate dEnd = this.DateTimeEnd.Value as iCalDate;
            if( dEnd != null ){
                evEnd = new iCalDateTime();
                evEnd.Date = dEnd;
                evEnd.Time = time;
            }

            // if( tStart != null ){
            //     if (Start <= date && End > date ) {
            //         return true;
            //     }
            // } else if( dStart != null ){
            //     if (Start <= date && date < End  ) {
            //         return true;
            //     }
            // }

            if( evStart >= dateTimeEnd ){
                return false;
            }

            if( evEnd <= dateTimeStart ){
                return false;
            }

            return true;
        }

        public List<iCalEvent> GetEventByDay( int year, int month, int day )
        {
            return this.GetEventByDay( year, month, day, TimeZoneInfo.Local );
        }
        
        public List<iCalEvent> GetEventByDay( int year, int month, int day,
                                              TimeZoneInfo timeZoneInfo )
        {
            iCalDate date = new iCalDate();
            date.Year = year;
            date.Month = month;
            date.Day = day;

            iCalTime time = new iCalTime();
            
            iCalDateTime dateTimeStart = new iCalDateTime();
            dateTimeStart.Date = date;
            dateTimeStart.Time = time;

            iCalDateTime dateTimeEnd = new iCalDateTime();
            dateTimeEnd.Date = date.NextDay();
            dateTimeEnd.Time = time;

            List<iCalEvent> ret = new List<iCalEvent>();
            iCalEvent previous = null;

            foreach( iCalEvent ev in this ){
                if( ev.IsExceptionDate() ){
                    continue;
                }

                if( ev.DateTimeStart != null ){
                    ev.DateTimeStart = (iCalDateTimeStart)
                        ev.DateTimeStart.ToLocal( timeZoneInfo );
                }

                if (previous != null &&
                    ev.DateTimeStart == previous.DateTimeStart) {
                    continue;
                }

                if (ev.dateTimeEnd != null) {
                    ev.dateTimeEnd = (iCalDateTimeEnd)
                        ev.dateTimeEnd.ToLocal( timeZoneInfo );
                }

                if( ev.RecurrenceId != null ){
                    ev.RecurrenceId = (iCalRecurrenceId)
                        ev.RecurrenceId.ToLocal( timeZoneInfo );
                }

                if( ev.OnDay( date ) ){
                    ret.Add( ev );
                    previous = ev;
                }
                if( ev.DateTimeStart.Value > dateTimeEnd ){
                    break;
                }
            }

            return ret;
        }
    }

    public class iCalEventEnum : IEnumerator<iCalEvent> {
        bool first = true;
        iCalEvent Original;
        bool rdatesEnumValid = false;
        IEnumerator<iCalDateTimeStart> rdatesEnum;
        iCalDateTimeStart rdatesDate = null;

        bool rrulesEnumValid = false;
        IEnumerator<iCalDateTimeStart> rrulesEnum;
        iCalDateTimeStart rrulesDate = null;
        


        Object IEnumerator.Current {
            get { return this.Current; }
        }
        
        iCalEvent IEnumerator<iCalEvent>.Current {
             get { return this.Current; }
        }

        public iCalEvent Current { get; set; }

        public iCalEventEnum(iCalEvent Original) {
            this.Original = Original;
        }

        public bool MoveNext()
        {
            if( first ){
                first = false;

                iCalEvent ev = (iCalEvent)this.Original.Clone();
                ev.Original = this.Original;

                this.Current = ev;
                return true;
            } else {

                iCalDateTimeStart dtstart = nextRDate();
                if( dtstart == null ){
                    return false;
                }
                
                iCalEvent ev =
                    this.Original.RecalcFromStartTime( dtstart );

                this.Current = ev;
                return true;
            }
        }

        iCalDateTimeStart nextRDate()
        {
            if( this.rdatesEnumValid == false ){
                this.rdatesEnum =
                    this.Original.GetSortedRecurrenceDateTimes().GetEnumerator();
                this.rdatesEnumValid = true;
            }

            if( this.rrulesEnumValid == false ){
                this.rrulesEnum =
                    new iCalRecurrenceRuleEnum( this.Original.DateTimeStart,
                                                this.Original.RRule );
                this.rrulesEnumValid = true;
            }

            iCalDateTimeStart ret = null;
            if( this.rdatesDate == null && this.rdatesEnum.MoveNext() ){
                this.rdatesDate = this.rdatesEnum.Current;
            }
            if( this.rrulesDate == null && this.rrulesEnum.MoveNext() ){
                this.rrulesDate = this.rrulesEnum.Current;
            }
            if( this.rdatesDate == null ){
                ret = this.rrulesDate;
                this.rrulesDate = null;
            } else if( this.rrulesDate == null ){
                ret = this.rdatesDate;
                this.rdatesDate = null;
            } else {
                // iCalDateTimeStart rrulesDate2 = this.rrulesDate;
                // iCalDateTimeStart rdatesDate2 = this.rdatesDate;
                // if( rrulesDate2.TimeZoneId != rdatesDate2.TimeZoneId ){
                //     rrulesDate2 = (iCalDateTimeStart)this.rrulesDate.ToUTC();
                //     rdatesDate2 = (iCalDateTimeStart)this.rdatesDate.ToUTC();
                // }
                //int result =
                //    this.Original.Compare( this.rrulesDate, this.rdatesDate );
                if( this.rrulesDate < this.rdatesDate ){
                // int result = this.rrulesDate.CompareTo(this.rdatesDate);
                // if( result < 0 ){
                    ret = this.rrulesDate;
                    this.rrulesDate = null;
                } else {
                    ret = this.rdatesDate;
                    this.rdatesDate = null;
                }
            }
            return ret;
        }

        public void Reset()
        {
            this.first = true;
            this.rdatesEnumValid = false;
            // this.timeRelatedEnumValid = false;
        }

        public void Dispose(){
            if( this.rdatesEnumValid ){
                this.rdatesEnum.Dispose();
            }
        }
    }

    public class iCalToDo : iCalEntry, IEnumerable<iCalEvent>
    {
        // todoc      = "BEGIN" ":" "VTODO" CRLF
        //              todoprop *alarmc
        //              "END" ":" "VTODO" CRLF
        // todoprop   = *(
        //            ;
        //            ; The following are REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            dtstamp / uid /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            class / completed / created / description /
        //            dtstart / geo / last-mod / location / organizer /
        //            percent / priority / recurid / seq / status /
        //            summary / url /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but SHOULD NOT occur more than once.
        //            ;
        //            rrule /
        //            ;
        //            ; Either 'due' or 'duration' MAY appear in
        //            ; a 'todoprop', but 'due' and 'duration'
        //            ; MUST NOT occur in the same 'todoprop'.
        //            ; If 'duration' appear in a 'todoprop',
        //            ; then 'dtstart' MUST also appear in
        //            ; the same 'todoprop'.
        //            ;
        //            due / duration /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            attach / attendee / categories / comment / contact /
        //            exdate / rstatus / related / resources /
        //            rdate / x-prop / iana-prop
        //            ;
        //            )
        public static String RepName = "vtodo";

        // original event if rrule exists or datetime conversion needed
        public iCalToDo Original = null;

        private iCalDurationProperty duration;
        public iCalDurationProperty Duration {
            set { this.duration = value; }
            get {
                if( this.duration != null ){
                    return this.duration ;
                } else if( this.DateTimeStart != null &&
                           this.dateTimeDue != null ){
                    iCalDurationProperty duration =
                        this.dateTimeDue - this.DateTimeStart;
                    return duration;
                }
                return null;
            }
        }
        public iCalGeographicPosition Geo;
        public iCalPriority Priority;
        public iCalLocation Location;

        private iCalDateTimeDue dateTimeDue;
        public iCalDateTimeDue DateTimeDue {
            set { this.dateTimeDue = value; }
            get {
                if( this.dateTimeDue != null ){
                    return this.dateTimeDue;
                } else if( this.DateTimeStart != null &&
                           this.duration != null ){

                    iCalDateTimeDue datetimedue =
                        new iCalDateTimeDue( this.DateTimeStart, this );

                    datetimedue.Value += this.Duration.Value;
                    return datetimedue;
                }
                return null;
            }
        }

        public List<iCalResources> ResourceList =
            new List<iCalResources>();
        
        public override Object Clone()
        {
            iCalToDo ret = (iCalToDo)base.Clone();

            ret.Original = this.Original;
            if( this.duration != null ){
                ret.duration = (iCalDurationProperty)this.duration.Clone();
            }
            if( this.Geo != null ){
                ret.Geo = (iCalGeographicPosition)this.Geo.Clone();
            }
            if( this.Priority != null ){
                ret.Priority = (iCalPriority)this.Priority.Clone();
            }
            if( this.Location != null ){
                ret.Location = (iCalLocation)this.Location.Clone();
            }
            if( this.dateTimeDue != null ){
                ret.dateTimeDue = (iCalDateTimeDue)this.dateTimeDue.Clone();
            }
            
            ret.ResourceList = new List<iCalResources>();
            foreach( iCalResources elem in this.ResourceList ){
                ret.ResourceList.Add( (iCalResources)elem.Clone() );
            }
            return ret;
        }

        public override void SetDurationProperty( iCalLineContent content ){
            this.Duration = new iCalDurationProperty( content, this );
        }

        public override void SetGeographicPosition( iCalLineContent content ){
            this.Geo = new iCalGeographicPosition( content, this );
        }

        public override void SetPriority( iCalLineContent content ){
            this.Priority = new iCalPriority( content, this );
        }

        public override void SetLocation( iCalLineContent content ){
            this.Location = new iCalLocation( content, this );
        }

        public override void SetDateTimeDue( iCalLineContent content ){
            this.DateTimeDue = new iCalDateTimeDue( content, this );
        }

        public override void SetResources( iCalLineContent content ){
            this.ResourceList.Add( new iCalResources( content, this ) );
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }

        IEnumerator<iCalEvent> IEnumerable<iCalEvent>.GetEnumerator()
        {
            return (IEnumerator<iCalEvent>) GetEnumerator();
        }

        public iCalToDoEnum GetEnumerator()
        {
            return new iCalToDoEnum( this );
        }

        public iCalToDo
            RecalcFromStartTime( iCalDateTimeStart dateTimeStart )
                                 //iCalTimeRelatedType timeRelated,
                                 // String timeZoneId )
        {
            iCalToDo ret = (iCalToDo)this.Clone();
            ret.Original = this;
            
            if( ret.DateTimeStart != null ){
                if( dateTimeStart.Value is iCalDate &&
                    ret.DateTimeStart.Value is iCalDateTime ) {

                    ((iCalDateTime)ret.DateTimeStart.Value).Date =
                        (iCalDate)dateTimeStart.Value;

                } else {
                    ret.DateTimeStart.Value = dateTimeStart.Value;
                    ret.DateTimeStart.TimeZoneId = dateTimeStart.TimeZoneId;
                }

                // ret.DateTimeStart.Component = ret; // how about other properties?

                if( ret.duration != null ){
                    // already copied by Clone(). 
                    ret.dateTimeDue = null;
                } else if( this.dateTimeDue != null ){
                    iCalDurationProperty duration = this.Duration;
                    
                    ret.dateTimeDue =
                        new iCalDateTimeDue( ret.DateTimeStart + duration,
                                             ret);
                    
                }
            }
            return ret;
        }

        public bool OnDay( iCalDate date, bool includesNoDateEntry )
        {
            if( this.Status != null &&
                this.Status.Value == iCalStatus.ValueType.Completed ){
                return false;
            }

            if( !includesNoDateEntry ){
                if( this.DateTimeStart == null && this.DateTimeDue == null ){
                    return false;
                }
            }

            iCalTime time = new iCalTime();

            iCalDateTime dateTimeStart = new iCalDateTime();
            dateTimeStart.Date = date;
            dateTimeStart.Time = time;


            iCalDateTime dateTimeEnd = new iCalDateTime();
            dateTimeEnd.Date = date.NextDay();
            dateTimeEnd.Time = time;

            iCalDateTime evStart = null;
            iCalDateTime evEnd = null;
            iCalDate dStart = null;
            iCalDate dEnd = null;
            
            if( this.DateTimeStart != null ){
                evStart = this.DateTimeStart.Value as iCalDateTime;
                dStart = this.DateTimeStart.Value as iCalDate;
                if( dStart != null ){
                    evStart = new iCalDateTime();
                    evStart.Date = dStart;
                    evStart.Time = time;
                }
            }
            if( this.DateTimeDue != null ){
                evEnd = this.DateTimeDue.Value as iCalDateTime;
                dEnd = this.DateTimeDue.Value as iCalDate;
                if( dEnd != null ){
                    evEnd = new iCalDateTime();
                    evEnd.Date = dEnd;
                    evEnd.Time = time;
                }
            }

            if( evStart != null && evStart >= dateTimeEnd ){
                return false;
            }

            if( evEnd != null && evEnd <= dateTimeStart ){
                return false;
            }

            return true;
        }

        public List<iCalToDo> GetToDoByDay( int year, int month, int day,
                                            bool includesNoDateEntry )
        {
            return this.GetToDoByDay( year, month, day, includesNoDateEntry,
                                      TimeZoneInfo.Local );
        }

        public List<iCalToDo> GetToDoByDay( int year, int month, int day,
                                            bool includesNoDateEntry,
                                            TimeZoneInfo timeZoneInfo )
        {
            iCalDate date = new iCalDate();
            date.Year = year;
            date.Month = month;
            date.Day = day;

            iCalTime time = new iCalTime();
            
            iCalDateTime dateTimeStart = new iCalDateTime();
            dateTimeStart.Date = date;
            dateTimeStart.Time = time;

            iCalDateTime dateTimeEnd = new iCalDateTime();
            dateTimeEnd.Date = date.NextDay();
            dateTimeEnd.Time = time;

            List<iCalToDo> ret = new List<iCalToDo>();
            iCalToDo previous = null;

            foreach( iCalToDo ev in this ){
                if( ev.IsExceptionDate() ){
                    continue;
                }

                if( ev.DateTimeStart != null ){
                    ev.DateTimeStart = (iCalDateTimeStart)
                        ev.DateTimeStart.ToLocal( timeZoneInfo );
                }

                if (previous != null &&
                    ev.DateTimeStart == previous.DateTimeStart) {
                    continue;
                }

                if (ev.dateTimeDue != null) {
                    ev.dateTimeDue = (iCalDateTimeDue)
                        ev.dateTimeDue.ToLocal( timeZoneInfo );
                }

                if( ev.OnDay( date, includesNoDateEntry ) ){
                    ret.Add( ev );
                    previous = ev;
                }
                if( ev.DateTimeStart != null &&
                    ev.DateTimeStart.Value > dateTimeEnd ){
                    break;
                }
            }
            return ret;
        }
    }

    public class iCalToDoEnum : IEnumerator<iCalToDo> {
        bool first = true;
        iCalToDo Original;
        bool rdatesEnumValid = false;
        IEnumerator<iCalDateTimeStart> rdatesEnum;
        iCalDateTimeStart rdatesDate = null;

        bool rrulesEnumValid = false;
        IEnumerator<iCalDateTimeStart> rrulesEnum;
        iCalDateTimeStart rrulesDate = null;


        Object IEnumerator.Current {
            get { return this.Current; }
        }
        
        iCalToDo IEnumerator<iCalToDo>.Current {
             get { return this.Current; }
        }

        public iCalToDo Current { get; set; }

        public iCalToDoEnum(iCalToDo Original) {
            this.Original = Original;
        }

        public bool MoveNext()
        {
            if( first ){
                first = false;

                iCalToDo ev = (iCalToDo)this.Original.Clone();
                ev.Original = this.Original;

                this.Current = ev;
                return true;
            } else {
                iCalDateTimeStart dtstart = nextRDate();
                if( dtstart == null ){
                    return false;
                }

                iCalToDo ev =
                    this.Original.RecalcFromStartTime( dtstart );
                this.Current = ev;
                return true;
            }
        }

        iCalDateTimeStart nextRDate()
        {
            if( this.rdatesEnumValid == false ){
                this.rdatesEnum =
                    this.Original.GetSortedRecurrenceDateTimes().GetEnumerator();
                this.rdatesEnumValid = true;
            }

            if( this.rrulesEnumValid == false ){
                this.rrulesEnum =
                    new iCalRecurrenceRuleEnum( this.Original.DateTimeStart,
                                                this.Original.RRule );
                this.rrulesEnumValid = true;
            }

            iCalDateTimeStart ret = null;
            if( this.rdatesDate == null && this.rdatesEnum.MoveNext() ){
                this.rdatesDate = this.rdatesEnum.Current;
            }
            if( this.rrulesDate == null && this.rrulesEnum.MoveNext() ){
                this.rrulesDate = this.rrulesEnum.Current;
            }
            if( this.rdatesDate == null ){
                ret = this.rrulesDate;
                this.rrulesDate = null;
            } else if( this.rrulesDate == null ){
                ret = this.rdatesDate;
                this.rdatesDate = null;
            } else {
                if( this.rrulesDate < this.rdatesDate ){
                // int result = this.rrulesDate.CompareTo(this.rdatesDate);
                // if( result < 0 ){
                    ret = this.rrulesDate;
                    this.rrulesDate = null;
                } else {
                    ret = this.rdatesDate;
                    this.rdatesDate = null;
                }
            }
            return ret;
        }

        public void Reset()
        {
            this.first = true;
            this.rdatesEnumValid = false;
            // this.timeRelatedEnumValid = false;
        }

        public void Dispose(){
            if( this.rdatesEnumValid ){
                this.rdatesEnum.Dispose();
            }
        }
    }

    public class iCalJournal : iCalEntry
    {
        // journalc   = "BEGIN" ":" "VJOURNAL" CRLF
        //              jourprop
        //              "END" ":" "VJOURNAL" CRLF
        // jourprop   = *(
        //            ;
        //            ; The following are REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            dtstamp / uid /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            class / created / dtstart /
        //            last-mod / organizer / recurid / seq /
        //            status / summary / url /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but SHOULD NOT occur more than once.
        //            ;
        //            rrule /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            attach / attendee / categories / comment /
        //            contact / description / exdate / related / rdate /
        //            rstatus / x-prop / iana-prop
        //            ;
        //            )
        public static String RepName = "vjournal";
    }

    public class iCalFreeBusy : iCalComponent
    {
       // freebusyc  = "BEGIN" ":" "VFREEBUSY" CRLF
       //              fbprop
       //              "END" ":" "VFREEBUSY" CRLF

       // fbprop     = *(
       //            ;
       //            ; The following are REQUIRED,
       //            ; but MUST NOT occur more than once.
       //            ;
       //            dtstamp / uid /
       //            ;
       //            ; The following are OPTIONAL,
       //            ; but MUST NOT occur more than once.
       //            ;
       //            contact / dtstart / dtend /
       //            organizer / url /
       //            ;
       //            ; The following are OPTIONAL,
       //            ; and MAY occur more than once.
       //            ;
       //            attendee / comment / freebusy / rstatus / x-prop /
       //            iana-prop
       //            ;
       //            )
        public static String RepName = "vfreebusy";

    }

    public class iCalTimeZone : iCalComponent
    {
        // timezonec  = "BEGIN" ":" "VTIMEZONE" CRLF
        //              *(
        //              ;
        //              ; 'tzid' is REQUIRED, but MUST NOT occur more
        //              ; than once.
        //              ;
        //              tzid /
        //              ;
        //              ; 'last-mod' and 'tzurl' are OPTIONAL,
        //              ; but MUST NOT occur more than once.
        //              ;
        //              last-mod / tzurl /
        //              ;
        //              ; One of 'standardc' or 'daylightc' MUST occur
        //              ; and each MAY occur more than once.
        //              ;
        //              standardc / daylightc /
        //              ;
        //              ; The following are OPTIONAL,
        //              ; and MAY occur more than once.
        //              ;
        //              x-prop / iana-prop
        //              ;
        //              )
        //              "END" ":" "VTIMEZONE" CRLF
        // tzid       = "TZID" tzidpropparam ":" [tzidprefix] text CRLF
        // tzidprefix        = "/"
        public static String RepName = "vtimezone";

        public enum ZoneType { Standard, DayLight }

        public iCalTimeZoneIdentifierProperty TimeZoneId;
        public List<iCalTimeZoneElement> Elements = new List<iCalTimeZoneElement>();

        public override void AddChild( iCalComponent child ){

            iCalTimeZoneElement element = child as iCalTimeZoneElement;
            if( element != null ){
                this.Elements.Add( element );
            }
        }

        public override void SetTimeZoneIdentifierProperty( iCalLineContent content ){
            this.TimeZoneId =
                new iCalTimeZoneIdentifierProperty( content, this );
        }

        public iCalDateTime ToUTC( iCalDateTime src ){
            iCalDateTime ret = (iCalDateTime)src.Clone();
            if( ret.Time == null || ret.Time.IsUTC == true ){
                return ret;
            } else {
                iCalDateTimeStart dtstart = new iCalDateTimeStart( this.Parent ) ;
                dtstart.Value = new iCalDateTime();
                
                iCalTimeZoneElement elem = null;
                foreach( iCalTimeZoneElement el in this.Elements ){
                    if( el.RRule != null && el.RRule.Value != null &&
                        el.RRule.Value.Until != null ){
                        if( el.RRule.Value.Until < src &&
                            dtstart.Value < el.RRule.Value.Until ){

                            dtstart.Value = el.RRule.Value.Until;
                            elem = el;
                            continue;
                        }
                    }

                    foreach( iCalDateTimeStart dt in el ){
                        if( src < dt.Value ){
                            break;
                        }
                        if( dtstart < dt ){
                            dtstart = dt;
                            elem = el;
                        }
                    }
                }
                // iCalTimeZoneElement elem = this.Elements[0]; /**/
                if (elem == null) {
                    throw new Exception( "TimeZone "  +
                        this.TimeZoneId.Value + " is not correct" );
                }
                iCalUTCOffset offset = elem.OffsetTo.Value;
                ret -= offset;
            }
            ret.Time.IsUTC = true;
            return ret;
        }
    }

    public class iCalTimeZoneElement
        : iCalComponent, IEnumerable<iCalDateTimeStart>
    {
        // standardc  = "BEGIN" ":" "STANDARD" CRLF
        //              tzprop
        //              "END" ":" "STANDARD" CRLF
        // daylightc  = "BEGIN" ":" "DAYLIGHT" CRLF
        //              tzprop
        //              "END" ":" "DAYLIGHT" CRLF
        // tzprop     = *(
        //              ;
        //              ; The following are REQUIRED,
        //              ; but MUST NOT occur more than once.
        //              ;
        //              dtstart / tzoffsetto / tzoffsetfrom /
        //              ;
        //              ; The following is OPTIONAL,
        //              ; but SHOULD NOT occur more than once.
        //              ;
        //              rrule /
        //              ;
        //              ; The following are OPTIONAL,
        //              ; and MAY occur more than once.
        //              ;
        //              comment / rdate / tzname / x-prop / iana-prop
        //              ;
        //              )
        public iCalTimeZone.ZoneType ZoneType;

        public iCalDateTimeStart DateTimeStart = null;
        public iCalTimeZoneOffsetTo OffsetTo = null;
        public iCalTimeZoneOffsetFrom OffsetFrom = null;

        public iCalRecurrenceRule RRule = null;
        public List<iCalRecurrenceDateTimes> RecurrenceDateList =
            new List<iCalRecurrenceDateTimes>();

        public iCalTimeZoneIdentifierProperty TimeZoneId = null;
        public iCalTimeZoneName TimeZoneName = null;
        
        // public iCalTimeZoneElement( iCalTimeZone timeZone,
        //                       iCalTimeZone.ZoneType zoneType ){
        //     this.TimeZone = timeZone;
        //     this.ZoneType = zoneType;
        // }

        public override void SetDateTimeStart( iCalLineContent content ){
            this.DateTimeStart = new iCalDateTimeStart( content, this );
        }

        public override void SetTimeZoneOffsetTo( iCalLineContent content ){
            this.OffsetTo = new iCalTimeZoneOffsetTo( content, this );
        }
        public override void SetTimeZoneOffsetFrom( iCalLineContent content ){
            this.OffsetFrom = new iCalTimeZoneOffsetFrom( content, this );
        }
        public override void SetRecurrenceRule( iCalLineContent content ){
            this.RRule = new iCalRecurrenceRule( content, this );
        }
        public override void SetRecurrenceDateTimes( iCalLineContent content ){
            this.RecurrenceDateList.Add( new iCalRecurrenceDateTimes( content, this ) );
        }

        public override void SetTimeZoneName( iCalLineContent content ){
            this.TimeZoneName = new iCalTimeZoneName( content, this );
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }

        IEnumerator<iCalDateTimeStart>
            IEnumerable<iCalDateTimeStart>.GetEnumerator()
        {
            return (IEnumerator<iCalDateTimeStart>) GetEnumerator();
        }

        public iCalTZElementEnum GetEnumerator()
        {
            return new iCalTZElementEnum( this );
        }
        public SortedSet<iCalDateTimeStart> GetSortedRecurrenceDateTimes()
        {
            SortedSet<iCalDateTimeStart> ret =
                new SortedSet<iCalDateTimeStart>();

            foreach( iCalRecurrenceDateTimes rdates in this.RecurrenceDateList )
            {
                foreach( iCalTimeRelatedType timeRelated in rdates.Values ){

                    iCalDateTimeStart dtstart = new iCalDateTimeStart( this );

                    dtstart.Value = (iCalTimeRelatedType)timeRelated.Clone();
                    dtstart.TimeZoneId = rdates.TimeZoneId;

                    ret.Add( dtstart );

                }
            }
            
            return ret;
        }
    }

    public class iCalTZElementEnum : IEnumerator<iCalDateTimeStart>
    {
        bool first = true;
        iCalTimeZoneElement original;

        bool rrulesEnumValid = false;
        IEnumerator<iCalDateTimeStart> rrulesEnum;
        iCalDateTimeStart rrulesDate = null;

        bool rdatesEnumValid = false;
        IEnumerator<iCalDateTimeStart> rdatesEnum;
        iCalDateTimeStart rdatesDate = null;

        Object IEnumerator.Current {
            get { return this.Current; }
        }

        iCalDateTimeStart IEnumerator<iCalDateTimeStart>.Current {
             get { return this.Current; }
        }

        public iCalDateTimeStart Current { get; set; }

        public iCalTZElementEnum( iCalTimeZoneElement element){
            this.original = element;
        }

        public bool MoveNext()
        {
            if( this.first ){

                if( this.original.DateTimeStart != null ){
                    this.Current =
                        (iCalDateTimeStart)this.original.DateTimeStart.Clone();
                    this.first = false;
                    return true;
                }
            } else {
                if( this.rdatesEnumValid == false ){
                    this.rdatesEnum =
                        this.original.GetSortedRecurrenceDateTimes().GetEnumerator();
                    this.rdatesEnumValid = true;
                }

                if( this.rrulesEnumValid == false ){
                    this.rrulesEnum =
                        new iCalRecurrenceRuleEnum( this.original.DateTimeStart,
                                                    this.original.RRule );
                    this.rrulesEnumValid = true;
                }
                iCalDateTimeStart ret = null;
                if( this.rdatesDate == null && this.rdatesEnum.MoveNext() ){
                    this.rdatesDate = this.rdatesEnum.Current;
                }
                if( this.rrulesDate == null && this.rrulesEnum.MoveNext() ){
                    this.rrulesDate = this.rrulesEnum.Current;
                }
                if( this.rdatesDate == null ){
                    ret = this.rrulesDate;
                    this.rrulesDate = null;
                } else if( this.rrulesDate == null ){
                    ret = this.rdatesDate;
                    this.rdatesDate = null;
                } else {
                    //int result =
                    //    this.Original.Compare( this.rrulesDate, this.rdatesDate );
                    if (this.rrulesDate < this.rdatesDate) {
                        ret = this.rrulesDate;
                        this.rrulesDate = null;
                    } else {
                        ret = this.rdatesDate;
                        this.rdatesDate = null;
                    }
                }
                if( ret != null ){
                    this.Current = ret;
                    return true;
                }
            }
            return false;
        }

        public void Reset(){
            this.first = true;
            this.rrulesEnumValid = false;
            this.rdatesEnumValid = false;
        }

        public void Dispose(){
            if( this.rrulesEnumValid ){
                this.rrulesEnum.Dispose();
                this.rrulesEnumValid = false;
            }
            if( this.rdatesEnumValid ){
                this.rdatesEnum.Dispose();
                this.rdatesEnumValid = false;
            }
        }
    }

    public class iCalStandard : iCalTimeZoneElement
    {
        public static String RepName = "standard";

        public iCalStandard() {
            this.ZoneType = iCalTimeZone.ZoneType.Standard;
        }
    }

    public class iCalDaylight : iCalTimeZoneElement
    {
        public static String RepName = "daylight";

        public iCalDaylight() {
            this.ZoneType = iCalTimeZone.ZoneType.DayLight;
        }
    }
    
    public class iCalAlarm : iCalComponent
    {
        // alarmc     = "BEGIN" ":" "VALARM" CRLF
        //              (audioprop / dispprop / emailprop)
        //              "END" ":" "VALARM" CRLF
        // audioprop  = *(
        //            ;
        //            ; 'action' and 'trigger' are both REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            action / trigger /
        //            ;
        //            ; 'duration' and 'repeat' are both OPTIONAL,
        //            ; and MUST NOT occur more than once each;
        //            ; but if one occurs, so MUST the other.
        //            ;
        //            duration / repeat /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            attach /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            x-prop / iana-prop
        //            ;
        //            )
        // dispprop   = *(
        //            ;
        //            ; The following are REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            action / description / trigger /
        //            ;
        //            ; 'duration' and 'repeat' are both OPTIONAL,
        //            ; and MUST NOT occur more than once each;
        //            ; but if one occurs, so MUST the other.
        //            ;
        //            duration / repeat /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            x-prop / iana-prop
        //            ;
        //            )
        // emailprop  = *(
        //            ;
        //            ; The following are all REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            action / description / trigger / summary /
        //            ;
        //            ; The following is REQUIRED,
        //            ; and MAY occur more than once.
        //            ;
        //            attendee /
        //            ;
        //            ; 'duration' and 'repeat' are both OPTIONAL,
        //            ; and MUST NOT occur more than once each;
        //            ; but if one occurs, so MUST the other.
        //            ;
        //            duration / repeat /
        //            ;
        //            ; The following are OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            attach / x-prop / iana-prop
        //            ;
        //            )
        public static String RepName = "valarm";
    }

    public class iCalUnkownComponent : iCalComponent
    {
        public String Name;
    }

    public class iCalComponentFactory
    {
        public iCalComponent Create( String componentName )
        {
            iCalComponent ret;
            
            componentName = componentName.ToLower();

            if( componentName == iCalendar.RepName ){
                ret = new iCalendar();
            } else if( componentName == iCalEvent.RepName ){
                ret = new iCalEvent();
            } else if( componentName ==iCalToDo.RepName ){
                ret = new iCalToDo();
            } else if( componentName ==iCalJournal.RepName ){
                ret = new iCalJournal();
            } else if( componentName ==iCalFreeBusy.RepName ){
                ret = new iCalFreeBusy();
            } else if( componentName ==iCalTimeZone.RepName ){
                ret = new iCalTimeZone();
            } else if( componentName ==iCalStandard.RepName ){
                ret = new iCalStandard();
            } else if( componentName ==iCalDaylight.RepName ){
                ret = new iCalDaylight();
            } else if( componentName ==iCalAlarm.RepName ){
                ret = new iCalAlarm();
            } else {
                ret = new iCalUnkownComponent();
                ( (iCalUnkownComponent)ret ).Name = componentName;
            }

            return ret;
        }
    }
}
