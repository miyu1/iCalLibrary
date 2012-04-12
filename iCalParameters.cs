// Copyright 2011 Miyako Komooka
using System;
using System.Collections.Generic;


// Component Properties, Defined in rfc5545 section 3.2
namespace iCalLibrary.Parameter
{
    // icalparameter = altrepparam       ; Alternate text representation
    //               / cnparam           ; Common name
    //               / cutypeparam       ; Calendar user type
    //               / delfromparam      ; Delegator
    //               / deltoparam        ; Delegatee
    //               / dirparam          ; Directory entry
    //               / encodingparam     ; Inline encoding
    //               / fmttypeparam      ; Format type
    //               / fbtypeparam       ; Free/busy time type
    //               / languageparam     ; Language for text
    //               / memberparam       ; Group or list membership
    //               / partstatparam     ; Participation status
    //               / rangeparam        ; Recurrence identifier range
    //               / trigrelparam      ; Alarm trigger relationship
    //               / reltypeparam      ; Relationship type
    //               / roleparam         ; Participation role
    //               / rsvpparam         ; RSVP expectation
    //               / sentbyparam       ; Sent by
    //               / tzidparam         ; Reference to time zone object
    //               / valuetypeparam    ; Property value data type
    //               / other-param
    // other-param   = (iana-param / x-param)
    // iana-param  = iana-token "=" param-value *("," param-value)
    // ; Some other IANA-registered iCalendar parameter.
    // x-param     = x-name "=" param-value *("," param-value)
    // ; A non-standard, experimental parameter.

    public class iCalParameter
    {
        public String Name = "";
        public List<String> Values = new List<String>();

        // Concatinated Values List
        public String Value()
        {
            String ret = "";
            bool first = true;
            foreach( String str in this.Values ){
                if( first ){
                    first = false;
                } else {
                    ret += ",";
                }
                ret += str;
            }
            return ret;
        }

        public static String RepName = "";
        // public static String[] ValueKeywords = { "" };

        public virtual String[] GetValueKeywords()
        {
            String[] list = { "" };
            return list;
        }

        public int GetValueType( String str ){
            String[] keywords = this.GetValueKeywords();

            for( int i = 0; i < keywords.Length; i++ ){
                if( str == keywords[i] ){
                    return i;
                }
            }
            return 0;
        }
    }

    public class iCalAlternateTextRepresentation : iCalParameter
    {
        // altrepparam = "ALTREP" "=" DQUOTE uri DQUOTE
        public static new String RepName = "altrep";

    }

    public class iCalCommonName : iCalParameter
    {
        // cnparam    = "CN" "=" param-value
        public static new String RepName = "cn";

    }

    public class iCalCalendarUserType : iCalParameter
    {
        // cutypeparam        = "CUTYPE" "="
        //                    ("INDIVIDUAL"   ; An individual
        //                   / "GROUP"        ; A group of individuals
        //                   / "RESOURCE"     ; A physical resource
        //                   / "ROOM"         ; A room resource
        //                   / "UNKNOWN"      ; Otherwise not known
        //                   / x-name         ; Experimental type
        //                   / iana-token)    ; Other IANA-registered
        //                                    ; type
        // ; Default is INDIVIDUAL
        public static new String RepName = "cutype";

        public enum ValueType { Individual=1, Group, Resource,
                          Room, Unknown }

        public override String[] GetValueKeywords() {
            String[] list = { "", "individual","group","resource","room","unknown" };
            return list; 
        }
    }

    public class iCalDelegators : iCalParameter
    {
        // delfromparam       = "DELEGATED-FROM" "=" DQUOTE cal-address
        //                      DQUOTE *("," DQUOTE cal-address DQUOTE)
        public static new String RepName = "delegated-from";
    }

    public class iCalDelegatees : iCalParameter
    {
        // deltoparam = "DELEGATED-TO" "=" DQUOTE cal-address DQUOTE
        //              *("," DQUOTE cal-address DQUOTE)
        public static new String RepName = "delegated-to";
    }

    public class iCalDirectoryEntryReference : iCalParameter
    {
        // dirparam   = "DIR" "=" DQUOTE uri DQUOTE
        public static new String RepName = "dir";
    }

    public class iCalInlineEncoding : iCalParameter
    {
        // encodingparam      = "ENCODING" "="
        //                    ( "8BIT"
        //    ; "8bit" text encoding is defined in [RFC2045]
        //                    / "BASE64"
        //    ; "BASE64" binary encoding format is defined in [RFC4648]
        //                    )
        public static new String RepName = "encoding";

        public enum ValueType { Bit8=1, Base64 }

        public override String[] GetValueKeywords() {
            String[] list = { "", "8bit","base64" };
            return list;
        }
    }

    public class iCalFormatType : iCalParameter
    {
        // fmttypeparam = "FMTTYPE" "=" type-name "/" subtype-name
        //                ; Where "type-name" and "subtype-name" are
        //                ; defined in Section 4.2 of [RFC4288].
        public static new String RepName = "fmttype";
    }

    public class iCalFreeBusyTimeType : iCalParameter
    {
        // fbtypeparam        = "FBTYPE" "=" ("FREE" / "BUSY"
        //                    / "BUSY-UNAVAILABLE" / "BUSY-TENTATIVE"
        //                    / x-name
        //          ; Some experimental iCalendar free/busy type.
        //                    / iana-token)
        //          ; Some other IANA-registered iCalendar free/busy type.
        public static new String RepName = "fbtype";

        public enum ValueType { Free=1, Busy, BusyUnavailable, BusyTentative }

        public override String[] GetValueKeywords() {
            String[] list = { "", "free","busy", "busy-unavailable", "busy-tentative" };
            return list;
                
        }

    }

    public class iCalLanguage : iCalParameter
    {
        // languageparam = "LANGUAGE" "=" language
        // language = Language-Tag
        //            ; As defined in [RFC5646].

        public static new String RepName = "language";
    }
    
    public class iCalGroupOrListMember : iCalParameter
    {
        // memberparam        = "MEMBER" "=" DQUOTE cal-address DQUOTE
        //                      *("," DQUOTE cal-address DQUOTE)

        public static new String RepName = "member";
    }

    public class  iCalParticipationStatus : iCalParameter
    {
        // partstatparam    = "PARTSTAT" "="
        //                   (partstat-event
        //                  / partstat-todo
        //                  / partstat-jour)
        // partstat-event   = ("NEEDS-ACTION"    ; Event needs action
        //                  / "ACCEPTED"         ; Event accepted
        //                  / "DECLINED"         ; Event declined
        //                  / "TENTATIVE"        ; Event tentatively
        //                                       ; accepted
        //                  / "DELEGATED"        ; Event delegated
        //                  / x-name             ; Experimental status
        //                  / iana-token)        ; Other IANA-registered
        //                                       ; status
        // ; These are the participation statuses for a "VEVENT".
        // ; Default is NEEDS-ACTION.
        // partstat-todo    = ("NEEDS-ACTION"    ; To-do needs action
        //                  / "ACCEPTED"         ; To-do accepted
        //                  / "DECLINED"         ; To-do declined
        //                  / "TENTATIVE"        ; To-do tentatively
        //                                       ; accepted
        //                  / "DELEGATED"        ; To-do delegated
        //                  / "COMPLETED"        ; To-do completed
        //                                       ; COMPLETED property has
        //                                       ; DATE-TIME completed
        //                  / "IN-PROCESS"       ; To-do in process of
        //                                       ; being completed
        //                  / x-name             ; Experimental status
        //                  / iana-token)        ; Other IANA-registered
        //                                       ; status
        // ; These are the participation statuses for a "VTODO".
        // ; Default is NEEDS-ACTION.
        // partstat-jour    = ("NEEDS-ACTION"    ; Journal needs action
        //                  / "ACCEPTED"         ; Journal accepted
        //                  / "DECLINED"         ; Journal declined
        //                  / x-name             ; Experimental status
        //                  / iana-token)        ; Other IANA-registered
        //                                       ; status
        // ; These are the participation statuses for a "VJOURNAL".
        // ; Default is NEEDS-ACTION.

        public static new String RepName = "partstat";

        public enum ValueType { NeedsAction=1, Accepted, Declined, Tentative,
                                Delegated, InProcess }

        public override String[] GetValueKeywords() {
            String[] list = 
                { "", "needs-action","accepted", "declined", "tentative",
                  "delegated", "in-process" };
            return list;
        }
    }

    public class iCalRecurrenceIdentifierRange : iCalParameter
    {
        // rangeparam = "RANGE" "=" "THISANDFUTURE"
        // ; To specify the instance specified by the recurrence identifier
        // ; and all subsequent recurrence instances.
       
        public static new String RepName = "range";
        
        public enum ValueType { ThisAndFuture=1 }

        public override String[] GetValueKeywords() {
            String[] list = { "", "thisandfuture" };
            return list;
        }
    }

    public class iCalAlarmTriggerRelationship : iCalParameter
    {
       // trigrelparam       = "RELATED" "="
       //                     ("START"       ; Trigger off of start
       //                    / "END")        ; Trigger off of end

        public static new String RepName = "related";
        
        public enum ValueType { Start=1, End }

        public override String[] GetValueKeywords() {
            String[] list = { "", "start", "end" };
            return list;
        }
    }

    public class iCalRelationshipType : iCalParameter
    {
        // reltypeparam       = "RELTYPE" "="
        //                     ("PARENT"    ; Parent relationship - Default
        //                    / "CHILD"     ; Child relationship
        //                    / "SIBLING"   ; Sibling relationship
        //                    / iana-token  ; Some other IANA-registered
        //                                  ; iCalendar relationship type
        //                    / x-name)     ; A non-standard, experimental
        //                                  ; relationship type

        public static new String RepName = "reltype";
        
        public enum ValueType { Parent=1, Child, Sibling }

        public override String[] GetValueKeywords() {
            String[] list = { "", "parent", "child", "sibling" };;
            return list;
        }
            
    }

    public class iCalParticipationRole : iCalParameter
    {
        // roleparam  = "ROLE" "="
        //             ("CHAIR"             ; Indicates chair of the
        //                                  ; calendar entity
        //            / "REQ-PARTICIPANT"   ; Indicates a participant whose
        //                                  ; participation is required
        //            / "OPT-PARTICIPANT"   ; Indicates a participant whose
        //                                  ; participation is optional
        //            / "NON-PARTICIPANT"   ; Indicates a participant who
        //                                  ; is copied for information
        //                                  ; purposes only
        //            / x-name              ; Experimental role
        //            / iana-token)         ; Other IANA role
        // ; Default is REQ-PARTICIPANT

        public static new String RepName = "role";
        
        public enum ValueType { Chair=1, ReqParticipant, OptParticipant, NonParticipant }

        public override String[] GetValueKeywords() {
            String[] list = { "", "chair", "req-participant", "opt-participant", "non-participant" };
            return list;
        }
    }

    public class iCalRSVPExpectation : iCalParameter
    {
        // rsvpparam = "RSVP" "=" ("TRUE" / "FALSE")
        // ; Default is FALSE

        public static new String RepName = "rsvp";

        public enum ValueType { True=1, False }

        public override String[] GetValueKeywords() {
            String[] list = { "true", "false" };
            return list;
        }
    }

    public class iCalSentBy : iCalParameter
    {
        // sentbyparam        = "SENT-BY" "=" DQUOTE cal-address DQUOTE
        public static new String RepName = "sent-by";
    }

    public class iCalTimeZoneIdentifierParameter : iCalParameter
    {
       // tzidparam  = "TZID" "=" [tzidprefix] paramtext
       // tzidprefix = "/"
        
        public static new String RepName = "tzid";
    }

    public class iCalValueDataType : iCalParameter
    {
        // valuetypeparam = "VALUE" "=" valuetype
        // valuetype  = ("BINARY"
        //            / "BOOLEAN"
        //            / "CAL-ADDRESS"
        //            / "DATE"
        //            / "DATE-TIME"
        //            / "DURATION"
        //            / "FLOAT"
        //            / "INTEGER"
        //            / "PERIOD"
        //            / "RECUR"
        //            / "TEXT"
        //            / "TIME"
        //            / "URI"
        //            / "UTC-OFFSET"
        //            / x-name
        //            ; Some experimental iCalendar value type.
        //            / iana-token)
        //            ; Some other IANA-registered iCalendar value type.

        public static new String RepName = "value";

        public enum ValueType { Binary=1, Boolean, CalAddress, Date, DateTime,
                Duration, Float, Integer, Period, Recur, Text,
                Time, URI, UTCOffset }


        public override String[] GetValueKeywords() {
            String[] list = 
                { "", "binary", "boolean", "cal-address", "date", "date-time",
                  "duration", "float", "integer", "period", "recur", "text",
                  "time", "uri", "utc-offset" };
            return list;
        }
    }

    public class iCalParameterFactory
    {
        public iCalParameter Create( String paramName  ){
            iCalParameter ret = null;

            if( paramName == iCalAlternateTextRepresentation.RepName ){
                ret = new iCalAlternateTextRepresentation();
            } else if( paramName == iCalCommonName.RepName ){
                ret = new iCalCommonName();
            } else if( paramName == iCalCalendarUserType.RepName ){
                ret = new iCalCalendarUserType();
            } else if( paramName == iCalDelegators.RepName ){
                ret = new iCalDelegators();
            } else if( paramName == iCalDelegatees.RepName ){
                ret = new iCalDelegatees();
            } else if( paramName == iCalDirectoryEntryReference.RepName ){
                ret = new iCalDirectoryEntryReference();
            } else if( paramName == iCalInlineEncoding.RepName ){
                ret = new iCalInlineEncoding();
            } else if( paramName == iCalFormatType.RepName ){
                ret = new iCalFormatType();
            } else if( paramName == iCalFreeBusyTimeType.RepName ){
                ret = new iCalFreeBusyTimeType();
            } else if( paramName == iCalLanguage.RepName ){
                ret = new iCalLanguage();
            } else if( paramName == iCalGroupOrListMember.RepName ){
                ret = new iCalGroupOrListMember();
            } else if( paramName == iCalParticipationStatus.RepName ){
                ret = new iCalParticipationStatus();
            } else if( paramName == iCalRecurrenceIdentifierRange.RepName ){
                ret = new iCalRecurrenceIdentifierRange();
            } else if( paramName == iCalAlarmTriggerRelationship.RepName ){
                ret = new iCalAlarmTriggerRelationship();
            } else if( paramName == iCalRelationshipType.RepName ){
                ret = new iCalRelationshipType();
            } else if( paramName == iCalParticipationRole.RepName ){
                ret = new iCalParticipationRole();
            } else if( paramName == iCalRSVPExpectation.RepName ){
                ret = new iCalRSVPExpectation();
            } else if( paramName == iCalSentBy.RepName ){
                ret = new iCalSentBy();
            }else if( paramName == iCalTimeZoneIdentifierParameter.RepName ){
                ret = new iCalTimeZoneIdentifierParameter();
            } else if( paramName == iCalValueDataType.RepName ){
                ret = new iCalValueDataType();
            } else {
                ret = new iCalParameter();
            }

            return ret;
        }
    }
}
