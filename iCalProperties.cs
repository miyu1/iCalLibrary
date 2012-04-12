// Copyright 2011 Miyako Komooka
using System;
using System.Collections;
using System.Collections.Generic;


// Component Properties, Defined in rfc5545 section 3.7, 3.8
namespace iCalLibrary.Property
{
    using DataType;
    using Parameter;
    using Component;

    public class iCalProperty /* : ICloneable */ {
        public iCalComponent Component;
        public iCalLineContent Content;

        public iCalProperty()
        {
            this.Content = null;
            this.Component = null;
        }

        public iCalProperty( iCalComponent component ) {
            this.Component = component;
            this.Content = null;
        }

        public iCalProperty(iCalLineContent content,
                            iCalComponent component )
        {
            this.Content = content;
            this.Component = component;
        }

        public virtual Object Clone()
        {
            iCalProperty ret = (iCalProperty)this.MemberwiseClone();

            return ret;
        }
    }

    // 3.7.1 
    public class iCalScale : iCalProperty {
        // calscale   = "CALSCALE" calparam ":" calvalue CRLF
        // calparam   = *(";" other-param)
        // calvalue   = "GREGORIAN"
        public static String RepName = "calscale";

        public iCalText Value = null;

        public iCalScale( iCalComponent component ) :base( component ) {}

        public iCalScale( iCalLineContent content, iCalComponent component )
            :base( content, component ) {
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalScale ret = (iCalScale)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            return ret;
        }
    }

    // 3.7.2
    public class iCalMethod : iCalProperty {
        // method     = "METHOD" metparam ":" metvalue CRLF
        // metparam   = *(";" other-param)
        // metvalue   = iana-token
        public static String RepName = "method";

        public iCalText Value = null;
        
        public iCalMethod( iCalComponent component ) : base( component ) {}

        public iCalMethod( iCalLineContent content, iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalMethod ret = (iCalMethod)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            return ret;
        }
    }

    // 3.7.3
    public class iCalProductIdentifier : iCalProperty {
        // prodid     = "PRODID" pidparam ":" pidvalue CRLF
        // pidparam   = *(";" other-param)
        // pidvalue   = text
        // ;Any text that describes the product and version
        // ;and that is generally assured of being unique.
        public static String RepName = "prodid";

        public iCalText Value = null;

        public iCalProductIdentifier( iCalComponent component )
            : base( component ) {}

        public iCalProductIdentifier( iCalLineContent content,
                                      iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalProductIdentifier ret = (iCalProductIdentifier)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            return ret;
        }
    }

    // 3.7.4
    public class iCalVersion : iCalProperty {
        // version    = "VERSION" verparam ":" vervalue CRLF
        // verparam   = *(";" other-param)
        // vervalue   = "2.0"         ;This memo
        //            / maxver
        //            / (minver ";" maxver)
        // minver     = <A IANA-registered iCalendar version identifier>
        // ;Minimum iCalendar version needed to parse the iCalendar object.
        // maxver     = <A IANA-registered iCalendar version identifier>
        // ;Maximum iCalendar version needed to parse the iCalendar object.
        
        public static String RepName = "version";

        public iCalText Value = null;

        public iCalVersion( iCalComponent component ) : base( component ) {}

        public iCalVersion( iCalLineContent content, iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalVersion ret = (iCalVersion)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.1.1
    public class iCalAttachment : iCalProperty {
        // attach     = "ATTACH" attachparam ( ":" uri ) /
        //              (
        //                ";" "ENCODING" "=" "BASE64"
        //                ";" "VALUE" "=" "BINARY"
        //                ":" binary
        //              )
        //              CRLF
        // attachparam = *(
        //             ;
        //             ; The following is OPTIONAL for a URI value,
        //             ; RECOMMENDED for a BINARY value,
        //             ; and MUST NOT occur more than once.
        //             ;
        //             (";" fmttypeparam) /
        //             ;
        //             ; The following is OPTIONAL,
        //             ; and MAY occur more than once.
        //             ;
        //             (";" other-param)
        //             ;
        //             )
        // binary     = *(4b-char) [b-end]
        // ; A "BASE64" encoded character string, as defined by [RFC4648].
        // b-end      = (2b-char "==") / (3b-char "=")
        // b-char = ALPHA / DIGIT / "+" / "/"
        public static String RepName = "attach";

        public bool IsURI = true; // if not URI, it is binary
        public String URI = null;
        public String Binary = null;
        public iCalValueDataType.ValueType ValueType = 0;
        public String Encoding = null;
        public String Format = null;

        public iCalAttachment( iCalComponent component ) : base( component ) {}

        public iCalAttachment( iCalLineContent content,
                               iCalComponent component )
            : base( content, component ) {

            if( content.Params.ContainsKey( iCalValueDataType.RepName ) ||
                content.Params.ContainsKey( iCalInlineEncoding.RepName ) ){
                this.IsURI = false;
                this.Binary = content.Value;

                if( content.Params.ContainsKey( iCalValueDataType.RepName ) ){
                    iCalParameter valueTypeParam =
                        content.Params[ iCalValueDataType.RepName ];
                    this.ValueType = (iCalValueDataType.ValueType)
                        valueTypeParam.GetValueType( valueTypeParam.Value() );
                }

                if( content.Params.ContainsKey( iCalInlineEncoding.RepName ) ){
                    iCalParameter encodingParam =
                        content.Params[ iCalInlineEncoding.RepName ];
                    this.Encoding = encodingParam.Value();
                }

            } else {
                this.IsURI = true;
                this.URI = content.Value;
            }

            
            if( content.Params.ContainsKey( iCalFormatType.RepName ) ){
                iCalParameter formatParam =
                    content.Params[ iCalFormatType.RepName ];

                this.Format = formatParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalAttachment ret = (iCalAttachment)base.Clone();

            ret.IsURI = this.IsURI;
            ret.URI = this.URI;
            ret.Binary = this.Binary;
            ret.ValueType = this.ValueType;
            ret.Encoding = this.Encoding;
            ret.Format = this.Format;

            return ret;
        }

    }

    // 3.8.1.2
    public class iCalCategories : iCalProperty {
        // categories = "CATEGORIES" catparam ":" text *("," text)
        //              CRLF

        // catparam   = *(
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" languageparam ) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        
        public static String RepName = "categories";

        public List<iCalText> Values = new List<iCalText>();
        public String Language = null;

        public iCalCategories( iCalComponent component )
            : base( component ) {}

        public iCalCategories( iCalLineContent content,
                               iCalComponent component )
            : base( content, component ) {
            this.Values = iCalText.ParseMultipleTexts( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalCategories ret = (iCalCategories)base.Clone();

            ret.Language = this.Language;
            ret.Values = new List<iCalText>();

            foreach( iCalText text in ret.Values ){
                ret.Values.Add( (iCalText)text.Clone() );
            }

            return ret;
        }

    }

    // 3.8.1.3
    public class iCalClassification : iCalProperty
    {
        // class      = "CLASS" classparam ":" classvalue CRLF
        // classparam = *(";" other-param)
        // classvalue = "PUBLIC" / "PRIVATE" / "CONFIDENTIAL" / iana-token
        //            / x-name
        // ;Default is PUBLIC
        public static String RepName = "class";

        public enum ValueType { Other=0, Public, Private, Confidential }

        public ValueType Type = 0;
        public iCalText Value = null;
        
        public iCalClassification( iCalComponent component )
            : base( component ) {}

        public iCalClassification( iCalLineContent content,
                                   iCalComponent component )
            : base( content, component )
        {
            this.Value = new iCalText( content.Value );

            String value = content.Value.ToLower();
            if( value == "public"){
                this.Type = ValueType.Public;
            } else if( value == "private" ){
                this.Type = ValueType.Private;
            } else if( value == "confidential" ){
                this.Type = ValueType.Confidential;
            } else {
                this.Type = ValueType.Other;
            }
        }

        public override Object Clone()
        {
            iCalClassification ret = (iCalClassification)base.Clone();
            ret.Type = this.Type;
            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.1.4
    public class iCalComment : iCalProperty {
        // comment    = "COMMENT" commparam ":" text CRLF
        // commparam  = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" altrepparam) / (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "comment";

        public iCalText Value = null;
        public String Language = null;
        public String AltRep = null;

        public iCalComment( iCalComponent component )
            : base( component ) {}

        public iCalComment( iCalLineContent content, iCalComponent component )
            : base( content, component ) {

            this.Value = new iCalText( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }

            if( content.Params.ContainsKey( iCalAlternateTextRepresentation.RepName ) ){
                iCalParameter altrepParam =
                    content.Params[ iCalAlternateTextRepresentation.RepName ];

                this.AltRep = altrepParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalComment ret = (iCalComment)base.Clone();
            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            ret.Language = this.Language;
            ret.AltRep = this.AltRep;

            return ret;
        }
    }

    // 3.8.1.5
    public class iCalDescription : iCalProperty {
        // description = "DESCRIPTION" descparam ":" text CRLF
        // descparam   = *(
        //             ;
        //             ; The following are OPTIONAL,
        //             ; but MUST NOT occur more than once.
        //             ;
        //             (";" altrepparam) / (";" languageparam) /
        //             ;
        //             ; The following is OPTIONAL,
        //             ; and MAY occur more than once.
        //             ;
        //             (";" other-param)
        //             ;
        //             )
        public static String RepName = "description";

        public iCalText Value = null;
        public String Language = null;
        public String AltRep = null;

        public iCalDescription( iCalComponent component )
            : base( component ) {}

        public iCalDescription( iCalLineContent content,
                                iCalComponent component )
            : base( content, component ) {

            this.Value = new iCalText( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }

            if( content.Params.ContainsKey( iCalAlternateTextRepresentation.RepName ) ){
                iCalParameter altrepParam =
                    content.Params[ iCalAlternateTextRepresentation.RepName ];

                this.AltRep = altrepParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalDescription ret = (iCalDescription)base.Clone();
            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            ret.Language = this.Language;
            ret.AltRep = this.AltRep;

            return ret;
        }
    }
    
    // 3.8.1.6
    public class iCalGeographicPosition : iCalProperty {
        // geo        = "GEO" geoparam ":" geovalue CRLF
        // geoparam   = *(";" other-param)
        // geovalue   = float ";" float
        // ;Latitude and Longitude components
        public static String RepName = "geo";

        public double Latitude = 0.0;
        public double Longitude = 0.0;

        public iCalGeographicPosition( iCalComponent component )
            : base( component ) {}
        
        public iCalGeographicPosition( iCalLineContent content,
                                       iCalComponent component )
            : base( content, component ) {
            String str = content.Value;
            int index = str.IndexOf( ";" );
            if( index > 0 ){
                this.Latitude = Double.Parse( str.Substring( 0, index ) );
                this.Longitude = Double.Parse( str.Substring( index+1 ) );
            } else {
                this.Latitude = Double.Parse( str );
                this.Longitude = 0.0;
            }
        }

        public override Object Clone()
        {
            iCalGeographicPosition ret = (iCalGeographicPosition)base.Clone();
            ret.Latitude= this.Latitude;
            ret.Longitude = this.Longitude;

            return ret;
        }
        
    }

    // 3.8.1.7
    public class iCalLocation : iCalProperty {
        // location   = "LOCATION"  locparam ":" text CRLF
        // locparam   = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" altrepparam) / (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "location";

        public iCalText Value = null;
        public String Language = null;
        public String AltRep = null;

        public iCalLocation( iCalComponent component )
            : base( component ) {}

        public iCalLocation( iCalLineContent content, iCalComponent component )
            : base( content, component ) {

            this.Value = new iCalText( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }


            if( content.Params.ContainsKey( iCalAlternateTextRepresentation.RepName ) ){
                iCalParameter altrepParam =
                    content.Params[ iCalAlternateTextRepresentation.RepName ];

                this.AltRep = altrepParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalLocation ret = (iCalLocation)base.Clone();
            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            ret.Language = this.Language;
            ret.AltRep = this.AltRep;

            return ret;
        }

    }

    // 3.8.1.8
    public class iCalPercentComplete : iCalProperty {
        // percent = "PERCENT-COMPLETE" pctparam ":" integer CRLF
        // pctparam   = *(";" other-param)
        public static String RepName = "percent-complete";

        public int Value = 0;

        public iCalPercentComplete( iCalComponent component )
            : base( component ) {}

        public iCalPercentComplete( iCalLineContent content,
                                    iCalComponent component )
            : base( content, component ) {

            this.Value = Int32.Parse( content.Value );
        }

        public override Object Clone()
        {
            iCalPercentComplete ret = (iCalPercentComplete)base.Clone();

            ret.Value = this.Value;

            return ret;
        }
    }

    // 3.8.1.9
    public class iCalPriority : iCalProperty {
        // priority   = "PRIORITY" prioparam ":" priovalue CRLF
        // ;Default is zero (i.e., undefined).
        // prioparam  = *(";" other-param)
        // priovalue   = integer       ;Must be in the range [0..9]
        //    ; All other values are reserved for future use.
        public static String RepName = "priority";

        public int Value = 0;

        public iCalPriority( iCalComponent component )
            : base( component ) {}

        public iCalPriority( iCalLineContent content,
                             iCalComponent component )
            : base( content, component ) {

            this.Value = Int32.Parse( content.Value );
        }
        public override Object Clone()

        {
            iCalPriority ret = (iCalPriority)base.Clone();

            ret.Value = this.Value;

            return ret;
        }
    }

    // 3.8.1.10
    public class iCalResources : iCalProperty {
        // resources  = "RESOURCES" resrcparam ":" text *("," text) CRLF
        // resrcparam = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" altrepparam) / (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "resources";

        public List<iCalText> Values = new List<iCalText>();
        public String Language = null;
        public String AltRep = null;

        public iCalResources( iCalComponent component )
            : base( component ) {}

        public iCalResources( iCalLineContent content,
                              iCalComponent component )
            : base( content, component ) {
            this.Values = iCalText.ParseMultipleTexts( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }

            if( content.Params.ContainsKey( iCalAlternateTextRepresentation.RepName ) ){
                iCalParameter altrepParam =
                    content.Params[ iCalAlternateTextRepresentation.RepName ];

                this.AltRep = altrepParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalResources ret = (iCalResources)base.Clone();


            ret.Values = new List<iCalText>();
            foreach( iCalText text in this.Values ){
                ret.Values.Add( (iCalText)text.Clone() );
            }

            ret.Language = this.Language;
            ret.AltRep = this.AltRep;

            return ret;
        }
    }

    // 3.8.1.11
    public class iCalStatus : iCalProperty
    {
        // status          = "STATUS" statparam ":" statvalue CRLF
        // statparam       = *(";" other-param)
        // statvalue       = (statvalue-event
        //                 /  statvalue-todo
        //                 /  statvalue-jour)
        // statvalue-event = "TENTATIVE"    ;Indicates event is tentative.
        //                 / "CONFIRMED"    ;Indicates event is definite.
        //                 / "CANCELLED"    ;Indicates event was cancelled.
        // ;Status values for a "VEVENT"
        // statvalue-todo  = "NEEDS-ACTION" ;Indicates to-do needs action.
        //                 / "COMPLETED"    ;Indicates to-do completed.
        //                 / "IN-PROCESS"   ;Indicates to-do in process of.
        //                 / "CANCELLED"    ;Indicates to-do was cancelled.
        // ;Status values for "VTODO".
        //  statvalue-jour  = "DRAFT"        ;Indicates journal is draft.
        //                  / "FINAL"        ;Indicates journal is final.
        //                  / "CANCELLED"    ;Indicates journal is removed.
        // ;Status values for "VJOURNAL".
        public static String RepName = "status";

        public enum ValueType { Other=0, Tentative, Confirmed, Cancelled,
                NeedsAction, Completed, InProcess,
                Draft, Final }
        public ValueType Value = 0;

        public iCalStatus( iCalComponent component ) : base( component ) {}

        public iCalStatus( iCalLineContent content, iCalComponent component )
            : base( content, component ) {

            String value = content.Value.ToLower();

            if( value == "tentative" ){
                this.Value = ValueType.Tentative;
            } else if( value == "confirmed" ){
                this.Value = ValueType.Confirmed;
            } else if( value == "cancelled" ){
                this.Value = ValueType.Cancelled;
            } else if( value == "needs-action" ){
                this.Value = ValueType.NeedsAction;
            } else if( value == "completed" ){
                this.Value = ValueType.Completed;
            } else if( value == "in-process" ){
                this.Value = ValueType.InProcess;
            } else if( value == "draft" ){
                this.Value = ValueType.Draft;
            } else if( value == "final" ){
                this.Value = ValueType.Final;
            } else {
                this.Value = ValueType.Other;
            }
        }

        public override Object Clone()
        {
            iCalStatus ret = (iCalStatus)base.Clone();

            ret.Value = this.Value;

            return ret;
        }
    }


    // 3.8.1.12
    public class iCalSummary : iCalProperty {
        // summary    = "SUMMARY" summparam ":" text CRLF
        // summparam  = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" altrepparam) / (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "summary";

        public iCalText Value = null;
        public String Language = null;
        public String AltRep = null;
        
        public iCalSummary( iCalComponent component )
            : base( component ) {}

        public iCalSummary( iCalLineContent content, iCalComponent component )
            : base( content, component ) {
            
            this.Value = new iCalText( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }

            if( content.Params.ContainsKey( iCalAlternateTextRepresentation.RepName ) ){
                iCalParameter altrepParam =
                    content.Params[ iCalAlternateTextRepresentation.RepName ];

                this.AltRep = altrepParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalSummary ret = (iCalSummary)base.Clone();
            
            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            ret.Language = this.Language;
            ret.AltRep = this.AltRep;

            return ret;
        }

    }

    // base class of DateTimeStart DateTimeEnd...
    public abstract class iCalDateTimeProperty
        : iCalProperty, IComparable<iCalDateTimeProperty>
    {
        // public Dictionary<String,iCalParameter> Params = null;
        // public iCalTimeZone TimeZone = null;
        public String TimeZoneId = null;

        // public iCalTimeRelatedType DateTime = null;
        public iCalTimeRelatedType Value = null;

        public iCalDateTimeProperty( iCalComponent component )
            : base( component ) {}

        public iCalDateTimeProperty( iCalDateTimeProperty prop,
                                     iCalComponent component )
            : base( component )
        {
            if (prop.TimeZoneId != null) {
                this.TimeZoneId = prop.TimeZoneId;
            } else {
                this.TimeZoneId = null;
            }

            if( prop.Value != null ){
                this.Value = (iCalTimeRelatedType)prop.Value.Clone();
            }
        }

        public iCalDateTimeProperty( iCalLineContent content,
                                     iCalComponent component )
            : base( content, component )
        {
            // this.Params = new Dictionary<String,iCalParameter>(content.Params);
            
            String key;

            key = iCalTimeZoneIdentifierParameter.RepName;
            if( content.Params.ContainsKey( key ) ){
                iCalParameter timeZoneParam = content.Params[ key ];

                this.TimeZoneId = timeZoneParam.Value();
            }

            key = iCalValueDataType.RepName;
            if( content.Params.ContainsKey( key ) ){
                iCalValueDataType valueParam =
                    (iCalValueDataType)content.Params[ key ];
                String value = valueParam.Value().ToLower();
                int tmp = valueParam.GetValueType(value);
                iCalValueDataType.ValueType valueType =
                    (iCalValueDataType.ValueType)valueParam.GetValueType( value );
                
                if( valueType == iCalValueDataType.ValueType.Date ){
                    this.Value = new iCalDate( content.Value );
                } else {
                    this.Value = new iCalDateTime( content.Value );
                }
            } else {
                iCalDateTime datetime = new iCalDateTime( content.Value );
                if( datetime.Time == null ){
                    this.Value = datetime.Date;
                } else {
                    this.Value = datetime;
                }
            }
        }

        public override Object Clone()
        {
            iCalDateTimeProperty ret = (iCalDateTimeProperty)base.Clone();

            ret.TimeZoneId = this.TimeZoneId;
            if( this.Value != null ){
                ret.Value = (iCalTimeRelatedType)this.Value.Clone();
            }

            return ret;
        }


        public iCalDateTimeProperty ToUTC()
        {
            iCalDateTimeProperty ret = (iCalDateTimeProperty)this.Clone();
            
            iCalDateTime datetime = this.Value as iCalDateTime;

            if( datetime != null ){
                if (datetime.Time != null && datetime.Time.IsUTC == false) {
                    if (this.TimeZoneId != null) {
                        // String id = this.TimeZoneId.ToString();
                        iCalTimeZone timeZone =
                            this.Component.GetTimeZoneById(this.TimeZoneId);

                        if (timeZone != null) {
                            ret.Value = timeZone.ToUTC(datetime);
                        }
                    } else {
                        datetime = (iCalDateTime)datetime.Clone();
                        datetime.Time.IsUTC = true;
                        ret.Value = datetime;
                    }
                }
            }
            ret.TimeZoneId = null;
            return ret;
        }

        public override int GetHashCode()
        {
            return (this.TimeZoneId.GetHashCode() +
                    this.Value.GetHashCode() );
        }

        public iCalDateTimeProperty ToLocal()
        {
            return this.ToLocal( TimeZoneInfo.Local );
        }

        public iCalDateTimeProperty ToLocal( TimeZoneInfo timeZoneInfo )
        {
            iCalDateTimeProperty ret = this.ToUTC();

            iCalDateTime datetime = ret.Value as iCalDateTime;

            if( datetime != null ){
                DateTime systemDateTime = (DateTime)datetime;

                systemDateTime =
                    TimeZoneInfo.ConvertTime( systemDateTime,
                                              timeZoneInfo );

                bool check =
                    timeZoneInfo.IsDaylightSavingTime( systemDateTime );

                datetime = new iCalDateTime( systemDateTime );
                datetime.Time.IsUTC = false;
                ret.Value = datetime;
            }

            return ret;
        }

        public int CompareTo( iCalDateTimeProperty other )
        {
            iCalDurationProperty ret = this - other;
            if( ret.Value.IsMinus ){
                return -1;
            } else if( ret.Value.IsZero() ){
                return 0;
            } else {
                return 1;
            }
        }

        public bool Equals( iCalDateTimeProperty other ){
            int ret = this.CompareTo( other );
            if( ret == 0 ){
                return true;
            }
            return false;
        }

        public override bool Equals(object obj) {
            iCalDateTimeProperty prop = obj as iCalDateTimeProperty;
            if( prop != null ){
                return this.Equals( prop );
            } else {
                return base.Equals(obj);
            }
        }    

        public static iCalDurationProperty
            operator - ( iCalDateTimeProperty date1,
                         iCalDateTimeProperty date2 )
        {
            if( date1.TimeZoneId != date2.TimeZoneId ){
                date1 = date1.ToUTC();
                date2 = date2.ToUTC();
            }

            iCalDurationDataType durationType =  date1.Value - date2.Value;
            iCalDurationProperty ret = new iCalDurationProperty( date1.Component );
            ret.Value = durationType;

            return ret;
        }

        public static iCalDateTimeProperty
            operator + ( iCalDateTimeProperty date1,
                         iCalDurationProperty duration )
        {
            iCalDateTimeProperty ret = (iCalDateTimeProperty)date1.Clone();
            ret.Value += duration.Value;

            return ret;
        }

        public static bool operator < ( iCalDateTimeProperty date1,
                                        iCalDateTimeProperty date2 ){
            int ret = date1.CompareTo( date2 );
            if( ret < 0 ){
                return true;
            }
            return false;
        }

        public static bool operator > ( iCalDateTimeProperty date1,
                                        iCalDateTimeProperty date2 ){
            int ret = date1.CompareTo( date2 );
            if( ret > 0 ){
                return true;
            }
            return false;
        }

        public static bool operator ==  ( iCalDateTimeProperty date1,
                                          iCalDateTimeProperty date2 )
        {
            if( ((Object)date1) == null ){
                if( ((Object)date2) == null ){
                    return true;
                }
            } else {
                if( ((Object)date2) == null ){
                    return false;
                } else {
                    if( date1.Equals( date2 ) ){
                        return true;
                    }
                }
            }
                
            return false;
        }

        public static bool operator != ( iCalDateTimeProperty date1,
                                         iCalDateTimeProperty date2 ){

            if( date1 == date2 ){
                return false;
            }
            return true;
        }

    }

    // 3.8.2.1
    public class iCalDateTimeCompleted : iCalProperty
    {
        // completed  = "COMPLETED" compparam ":" date-time CRLF
        // compparam  = *(";" other-param)
        public static String RepName = "completed";

        iCalDateTime Value = null;

        public iCalDateTimeCompleted( iCalComponent component )
            : base( component ) {}

        public iCalDateTimeCompleted( iCalLineContent content,
                                      iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalDateTime( content.Value );
        }

        public override Object Clone()
        {
            iCalDateTimeCompleted ret = (iCalDateTimeCompleted)base.Clone();
            
            if( this.Value != null ){
                ret.Value = (iCalDateTime)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.2.2
    public class iCalDateTimeEnd : iCalDateTimeProperty
    {
        // dtend      = "DTEND" dtendparam ":" dtendval CRLF
        // dtendparam = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" ("DATE-TIME" / "DATE")) /
        //            (";" tzidparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // dtendval   = date-time / date

        public static String RepName = "dtend";

        public iCalDateTimeEnd( iCalComponent component )
            : base( component ){}

        public iCalDateTimeEnd( iCalLineContent content,
                                iCalComponent component )
            : base( content, component ){}

        public iCalDateTimeEnd( iCalDateTimeProperty prop,
                                iCalComponent component )
            : base( prop, component ) {}

    }

    // 3.8.2.3
    public class iCalDateTimeDue : iCalDateTimeProperty
    {
        // due        = "DUE" dueparam ":" dueval CRLF
        // dueparam   = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" ("DATE-TIME" / "DATE")) /
        //            (";" tzidparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // dueval     = date-time / date
        public static String RepName = "due";
         
        public iCalDateTimeDue( iCalComponent component )
            : base( component ){}

        public iCalDateTimeDue( iCalLineContent content,
                                iCalComponent component )
            : base( content, component ){}

        public iCalDateTimeDue( iCalDateTimeProperty prop,
                                iCalComponent component )
            : base( prop, component ) {}
    }

    // 3.8.2.4
    public class iCalDateTimeStart : iCalDateTimeProperty
    {
        // dtstart    = "DTSTART" dtstparam ":" dtstval CRLF
        // dtstparam  = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" ("DATE-TIME" / "DATE")) /
        //            (";" tzidparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // dtstval    = date-time / date
        // ;Value MUST match value type
        public static String RepName = "dtstart";

        public iCalDateTimeStart( iCalComponent component )
            : base( component ){}

        public iCalDateTimeStart( iCalLineContent content,
                                  iCalComponent component )
            : base( content, component ){}

        public override bool Equals( Object obj ){
            iCalDateTimeStart date2 = obj as iCalDateTimeStart;

            if( date2 == null ){
                return false;
            }

            if( this.TimeZoneId != date2.TimeZoneId ){
                return false;
            }

            if( this.Value != date2.Value ){
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static bool operator == ( iCalDateTimeStart date1,
                                         iCalDateTimeStart date2 ){

            if( ((Object)date1) == null ){
                if( ((Object)date2) == null ){
                    return true;
                }
            } else {
                if( ((Object)date2) == null ){
                    return false;
                } else {
                    if( date1.Equals( date2 ) ){
                        return true;
                    }
                }
            }
                
            return false;
        }

        public static bool operator != ( iCalDateTimeStart date1,
                                         iCalDateTimeStart date2 ){

            if( date1 == date2 ){
                return false;
            }
            return true;
        }
    }

    // 3.8.2.5
    public class iCalDurationProperty : iCalProperty
    {
        // duration   = "DURATION" durparam ":" dur-value CRLF
        //              ;consisting of a positive duration of time.
        // durparam   = *(";" other-param)
        public static String RepName = "duration";

        public iCalDurationDataType Value = null;
        
        public iCalDurationProperty( iCalComponent component )
            : base( component ){}

        public iCalDurationProperty( iCalLineContent content,
                                     iCalComponent component )
            : base( content, component ){
            this.Value = new iCalDurationDataType( content.Value );
        }

        public override Object Clone()
        {
            iCalDurationProperty ret = (iCalDurationProperty)base.Clone();
            
            if( this.Value != null ){
                ret.Value = (iCalDurationDataType)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.2.6
    public class iCalFreeBusyTime : iCalProperty
    {
        // freebusy   = "FREEBUSY" fbparam ":" fbvalue CRLF
        // fbparam    = *(
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" fbtypeparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // fbvalue    = period *("," period)
        // ;Time value MUST be in the UTC time format.
        public static String RepName = "freebusy";

        List<iCalPeriod> Periods = new List<iCalPeriod>();
        iCalFreeBusyTimeType.ValueType FreeBusyType = 0;
        
        public iCalFreeBusyTime( iCalComponent component )
            : base( component ) {}

        public iCalFreeBusyTime( iCalLineContent content,
                                 iCalComponent component )
            : base( content, component ) {

            String str = content.Value;
            String str1 = "";
            int index;

            while( true ){
                index = str.IndexOf( "," );
                if( index < 0 ){
                    this.Periods.Add( new iCalPeriod( str ) );
                    break;
                } else {
                    str1 = str.Substring( 0, index );
                    str = str.Substring( index + 1 );
                    this.Periods.Add( new iCalPeriod( str1 ) );
                }
            }

            if( content.Params.ContainsKey( iCalFreeBusyTimeType.RepName ) ){
                iCalParameter fbParam =
                    content.Params[ iCalFreeBusyTimeType.RepName ];
                int value = fbParam.GetValueType( fbParam.Value() );
                this.FreeBusyType = (iCalFreeBusyTimeType.ValueType)value;
            } else {
                this.FreeBusyType = 0;
            }
        }

        public override Object Clone()
        {
            iCalFreeBusyTime ret = (iCalFreeBusyTime)base.Clone();
            
            ret.Periods = new List<iCalPeriod>();
            foreach( iCalPeriod period in this.Periods ){
                ret.Periods.Add( (iCalPeriod)period.Clone() );
            }

            ret.FreeBusyType = this.FreeBusyType;

            return ret;
        }
    }

    // 3.8.2.7
    public class iCalTransparency : iCalProperty
    {
       // transp     = "TRANSP" transparam ":" transvalue CRLF
       // transparam = *(";" other-param)
       // transvalue = "OPAQUE"
       //             ;Blocks or opaque on busy time searches.
       //             / "TRANSPARENT"
       //             ;Transparent on busy time searches.
       // ;Default value is OPAQUE
        public static String RepName = "transp";

        public enum ValueType { Other=0, Opaque, Transparent }
        public ValueType Value = 0;

        public iCalTransparency( iCalComponent component )
            : base( component ) {}

        public iCalTransparency( iCalLineContent content,
                                 iCalComponent component )
            : base( content, component ) {
            String value = content.Value.ToLower();
            if( value == "opaque" ){
                this.Value = ValueType.Opaque;
            } else if( value == "transparent" ){
                this.Value = ValueType.Transparent;
            } else {
                this.Value = ValueType.Other;
            }
        }

        public override Object Clone()
        {
            iCalTransparency ret = (iCalTransparency)base.Clone();

            ret.Value = this.Value;

            return ret;
        }
    }

    // 3.8.3.1
    public class iCalTimeZoneIdentifierProperty : iCalProperty
    {
        // tzid       = "TZID" tzidpropparam ":" [tzidprefix] text CRLF
        // tzidpropparam      = *(";" other-param)
        // ;tzidprefix        = "/"
        public static String RepName = "tzid";

        public String Value = null;

        public iCalTimeZoneIdentifierProperty( iCalComponent component )
            : base( component ) {}

        public iCalTimeZoneIdentifierProperty( iCalLineContent content,
                                               iCalComponent component )
            : base( content, component )
        {
            this.Value = content.Value;
        }

        public override Object Clone()
        {
            iCalTimeZoneIdentifierProperty ret = (iCalTimeZoneIdentifierProperty)base.Clone();

            ret.Value = this.Value;

            return ret;
        }
    }
    
    // 3.8.3.2
    public class iCalTimeZoneName : iCalProperty
    {
        // tzname     = "TZNAME" tznparam ":" text CRLF
        // tznparam   = *(
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "tzname";

        public iCalText Value = null;

        public iCalTimeZoneName( iCalComponent component )
            : base( component ) {}

        public iCalTimeZoneName( iCalLineContent content,
                                 iCalComponent component )
            : base( content, component )
        {
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalTimeZoneName ret = (iCalTimeZoneName)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.3.3
    public class iCalTimeZoneOffsetFrom : iCalProperty
    {
        // tzoffsetfrom       = "TZOFFSETFROM" frmparam ":" utc-offset
        //                       CRLF
        // frmparam   = *(";" other-param)

        public static String RepName = "tzoffsetfrom";

        public iCalUTCOffset Value = null;
        
        public iCalTimeZoneOffsetFrom( iCalComponent component )
            : base( component ) {}

        public iCalTimeZoneOffsetFrom( iCalLineContent content,
                                       iCalComponent component )
            : base( content, component )
        {
            this.Value = new iCalUTCOffset( content.Value );
        }

        public override Object Clone()
        {
            iCalTimeZoneOffsetFrom ret = (iCalTimeZoneOffsetFrom)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalUTCOffset)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.3.4
    public class iCalTimeZoneOffsetTo : iCalProperty
    {
        // tzoffsetto = "TZOFFSETTO" toparam ":" utc-offset CRLF
        // toparam    = *(";" other-param)

        public static String RepName = "tzoffsetto";

        public iCalUTCOffset Value = null;
        
        public iCalTimeZoneOffsetTo( iCalComponent component )
            : base( component ) {}

        public iCalTimeZoneOffsetTo( iCalLineContent content,
                                     iCalComponent component )
            : base( content, component )
        {
            this.Value = new iCalUTCOffset( content.Value );
        }

        public override String ToString(){
            return this.Value.ToString();
        }

        public override Object Clone()
        {
            iCalTimeZoneOffsetTo ret = (iCalTimeZoneOffsetTo)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalUTCOffset)this.Value.Clone();
            }

            return ret;
        }
    }

    // 3.8.3.5
    public class iCalTimeZoneURL : iCalProperty
    {
        // tzurl      = "TZURL" tzurlparam ":" uri CRLF
        // tzurlparam = *(";" other-param)
        public static String RepName = "tzurl";

        public String Value;

        public iCalTimeZoneURL( iCalComponent component )
            : base( component ) {}

        public iCalTimeZoneURL( iCalLineContent content,
                                iCalComponent component )
            : base( content, component )
        {
            this.Value =  content.Value;
        }

        public override Object Clone()
        {
            iCalTimeZoneURL ret = (iCalTimeZoneURL)base.Clone();

            ret.Value = this.Value;

            return ret;
        }
    }
        
    // 3.8.4.1
    public class iCalAttendee : iCalProperty
    {
        // attendee   = "ATTENDEE" attparam ":" cal-address CRLF
        // attparam   = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" cutypeparam) / (";" memberparam) /
        //            (";" roleparam) / (";" partstatparam) /
        //            (";" rsvpparam) / (";" deltoparam) /
        //            (";" delfromparam) / (";" sentbyparam) /
        //            (";" cnparam) / (";" dirparam) /
        //            (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "attendee";

        public String Value = null;

        public iCalCalendarUserType.ValueType TypeOfUserType = 0;
        public String UserType = null;

        public String Member = null;

        public iCalParticipationRole.ValueType RoleType = 0;
        public String Role = null;

        public iCalParticipationStatus.ValueType ParticipationStatusType = 0;
        public String ParticipationStatus = null;

        public iCalRSVPExpectation.ValueType RSVPType = 0;
        
        public List<String> Delegatees = new List<String>();
        public List<String> Delegators = new List<String>();

        public String SentBy = null;
        public String CommonName = null;
        public String Directory = null;
        public String Language = null;

        public iCalAttendee( iCalComponent component )
            : base( component ) {}

        public iCalAttendee( iCalLineContent content,
                             iCalComponent component )
            : base( content, component ) {

            this.Value = content.Value;

            String key; int valueType;
            iCalParameter param;
            
            key = iCalCalendarUserType.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.UserType = param.Value();
                valueType = param.GetValueType( this.UserType );
                this.TypeOfUserType =
                    (iCalCalendarUserType.ValueType)valueType;
            }

            key = iCalGroupOrListMember.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Member = param.Value();
            }

            key = iCalParticipationRole.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Role = param.Value();
                valueType = param.GetValueType( this.Role );
                this.RoleType = (iCalParticipationRole.ValueType)valueType;
            }

            key = iCalParticipationStatus.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];

                this.ParticipationStatus = param.Value();
                valueType = param.GetValueType( this.ParticipationStatus );
                this.ParticipationStatusType =
                    (iCalParticipationStatus.ValueType)valueType;
            }

            key = iCalRSVPExpectation.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                valueType = param.GetValueType( param.Value() );

                this.RSVPType = (iCalRSVPExpectation.ValueType)valueType;
            }

            key = iCalDelegatees.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Delegatees = param.Values;
            }

            key = iCalDelegators.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Delegators = param.Values;
            }
            
            key = iCalSentBy.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.SentBy = param.Value();
            }

            key = iCalCommonName.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.CommonName = param.Value();
            }
            key = iCalDirectoryEntryReference.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Directory = param.Value();
            }
            key = iCalLanguage.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Language = param.Value();
            }
        }

        public override Object Clone()
        {
            iCalAttendee ret = (iCalAttendee)base.Clone();

            ret.Value = this.Value;
            ret.TypeOfUserType = this.TypeOfUserType;
            ret.UserType = this.UserType;
            ret.Member = this.Member;
            ret.RoleType = this.RoleType;
            ret.Role = this.Role;
            ret.ParticipationStatusType = this.ParticipationStatusType;
            ret.ParticipationStatus = this.ParticipationStatus;
            ret.RSVPType = this.RSVPType;
                
            ret.Delegatees = new List<String>();
            foreach( String delegatee in this.Delegatees ){
                ret.Delegatees.Add( delegatee );
            }

            ret.Delegators = new List<String>();
            foreach( String delegator in this.Delegators ){
                ret.Delegatees.Add( delegator );
            }

            ret.SentBy = this.SentBy;
            ret.CommonName = this.CommonName;
            ret.Directory = this.Directory;
            ret.Language = this.Language;

            return ret;
        }
    }

    // 3.8.4.2
    public class iCalContact : iCalProperty
    {
        // contact    = "CONTACT" contparam ":" text CRLF
        // contparam  = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" altrepparam) / (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "contact";
    

        public iCalText Value = null;
        public String Language = null;
        public String AltRep = null;

        public iCalContact( iCalComponent component )
            : base( component ) {}

        public iCalContact( iCalLineContent content,
                            iCalComponent component )
            : base( content, component ) {

            this.Value = new iCalText( content.Value );

            if( content.Params.ContainsKey( iCalLanguage.RepName ) ){
                iCalParameter langParam =
                    content.Params[ iCalLanguage.RepName ];
                this.Language = langParam.Value();
            }

            if( content.Params.ContainsKey( iCalAlternateTextRepresentation.RepName ) ){
                iCalParameter altrepParam =
                    content.Params[ iCalAlternateTextRepresentation.RepName ];

                this.AltRep = altrepParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalContact ret = (iCalContact)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            ret.Language = this.Language;
            ret.AltRep = this.AltRep;

            return ret;
        }
    }

    // 3.8.4.3
    public class iCalOrganizer : iCalProperty
    {
        // organizer  = "ORGANIZER" orgparam ":"
        //              cal-address CRLF
        // orgparam   = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" cnparam) / (";" dirparam) / (";" sentbyparam) /
        //            (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "organizer";

        public String Value = null;
        public String CommonName = null;
        public String Directory = null;
        public String SentBy = null;
        public String Language = null;

        public iCalOrganizer( iCalComponent component )
            : base( component ) {}

        public iCalOrganizer( iCalLineContent content,
                              iCalComponent component )
            : base( content, component ) {

            this.Value = content.Value;

            String key;
            iCalParameter param;

            key = iCalCommonName.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.CommonName = param.Value();
            }

            key = iCalDirectoryEntryReference.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Directory = param.Value();
            }

            key = iCalSentBy.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.SentBy = param.Value();
            }

            key = iCalLanguage.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Language = param.Value();
            }
        }

        public override Object Clone()
        {
            iCalOrganizer ret = (iCalOrganizer)base.Clone();

            ret.Value = this.Value;
            ret.CommonName = this.CommonName;
            ret.Directory = this.Directory;
            ret.SentBy = this.SentBy;
            ret.Language = this.Language;

            return ret;
        }
    }

    // 3.8.4.4
    public class iCalRecurrenceId : iCalDateTimeProperty
    {
        // recurid    = "RECURRENCE-ID" ridparam ":" ridval CRLF
        // ridparam   = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" ("DATE-TIME" / "DATE")) /
        //            (";" tzidparam) / (";" rangeparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // ridval     = date-time / date
        // ;Value MUST match value type
        public static String RepName = "recurrence-id";

        public bool RangeThisAndFuture = false;

        public iCalRecurrenceId( iCalComponent component )
            : base( component ) {}

        public iCalRecurrenceId( iCalLineContent content,
                                 iCalComponent component )
            : base( content, component ) {
            
            iCalParameter param;

            this.RangeThisAndFuture = false;
            String key = iCalRecurrenceIdentifierRange.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                iCalRecurrenceIdentifierRange.ValueType recidType =
                    (iCalRecurrenceIdentifierRange.ValueType)
                    param.GetValueType( param.Value() );

                if( recidType ==
                    iCalRecurrenceIdentifierRange.ValueType.ThisAndFuture ){
                    this.RangeThisAndFuture = true;
                }
            }
        }

        public override Object Clone()
        {
            iCalRecurrenceId ret = (iCalRecurrenceId)base.Clone();

            ret.RangeThisAndFuture = this.RangeThisAndFuture;

            return ret;
        }
    }

    // 3.8.4.5
    public class iCalRelatedTo : iCalProperty
    {
        // related    = "RELATED-TO" relparam ":" text CRLF
        // relparam   = *(
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" reltypeparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        public static String RepName = "related-to";

        public iCalText Value = null;

        public String Relation = null;
        public iCalRelationshipType.ValueType  RelationType = 0;

        public iCalRelatedTo( iCalComponent component )
            : base( component ){}

        public iCalRelatedTo( iCalLineContent content,
                              iCalComponent component )
            : base( content, component ){

            this.Value = new iCalText( content.Value );

            this.RelationType = 0;
            
            int valueType;
            iCalParameter param;
            String key = iCalRelationshipType.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                this.Relation = param.Value();
                param = content.Params[ key ];
                valueType = param.GetValueType( this.Relation );
                this.RelationType = (iCalRelationshipType.ValueType)valueType;
            }
        }

        public override Object Clone()
        {
            iCalRelatedTo ret = (iCalRelatedTo)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            
            ret.Relation = this.Relation;
            ret.RelationType = this.RelationType;

            return ret;
        }
    }

    // 3.8.4.6
    public class iCalURL : iCalProperty
    {
        // url        = "URL" urlparam ":" uri CRLF
        // urlparam   = *(";" other-param)
        public static String RepName = "url";

        public String Value = null;

        public iCalURL( iCalComponent component ) : base( component ){}

        public iCalURL( iCalLineContent content,
                        iCalComponent component )
            : base( content, component ){
            this.Value = content.Value;
        }

        public override Object Clone()
        {
            iCalURL ret = (iCalURL)base.Clone();

            ret.Value = this.Value;
            
            return ret;
        }
    }

    // 3.8.4.7
    public class iCalUID : iCalProperty
    {
       // uid        = "UID" uidparam ":" text CRLF
       // uidparam   = *(";" other-param)
        public static String RepName = "uid";

        public iCalText Value = null;

        public iCalUID( iCalComponent component ) : base( component ){}

        public iCalUID( iCalLineContent content,
                        iCalComponent component )
            : base( content, component ){
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalUID ret = (iCalUID)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            
            return ret;
        }
    }

    // 3.8.5.1
    public class iCalExceptionDateTimes : iCalProperty
    {
        // exdate     = "EXDATE" exdtparam ":" exdtval *("," exdtval) CRLF
        // exdtparam  = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" ("DATE-TIME" / "DATE")) /
        //            ;
        //            (";" tzidparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // exdtval    = date-time / date
        // ;Value MUST match value type
        public static String RepName = "exdate";

        public List<iCalTimeRelatedType> Values =
            new List<iCalTimeRelatedType>();
        public String TimeZoneId = null;

        public iCalExceptionDateTimes( iCalComponent component )
            : base( component ){}


        public iCalExceptionDateTimes( iCalLineContent content,
                                       iCalComponent component )
            : base( content, component ){
            
            String key;
            iCalParameter param;

            iCalValueDataType.ValueType dataType = 0;
            key = iCalValueDataType.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                dataType = (iCalValueDataType.ValueType)
                    param.GetValueType( param.Value() );
            }

            String str = content.Value;
            String str1 = "";
            int index;
            iCalDateTime datetime;
            while( str.Length > 0 ){
                index = str.IndexOf( "," );
                if( index >= 0 ){
                    str1 = str.Substring( 0, index );
                    str = str.Substring( index + 1 );
                } else {
                    str1 = str;
                    str = "";
                }
                
                if( dataType == iCalValueDataType.ValueType.DateTime ){
                    this.Values.Add( new iCalDateTime( str1 ) );
                } else if( dataType == iCalValueDataType.ValueType.Date ){
                    this.Values.Add( new iCalDate( str1 ) );
                } else {
                    datetime = new iCalDateTime( str1 );
                    if( datetime.Time == null ){
                        this.Values.Add( datetime.Date );
                    } else {
                        this.Values.Add( datetime );
                    }
                }
            }
            
            key = iCalTimeZoneIdentifierParameter.RepName;
            if( content.Params.ContainsKey( key ) ){
                iCalParameter timeZoneParam = content.Params[ key ];

                this.TimeZoneId = timeZoneParam.Value();
            }
        }

        public iCalExceptionDateTimes ToUTC()
        {
            iCalExceptionDateTimes ret = (iCalExceptionDateTimes)this.Clone();
            iCalTimeZone timezone =
                this.Component.GetTimeZoneById( this.TimeZoneId );

            ret.Values = new List<iCalTimeRelatedType>();
            foreach( iCalTimeRelatedType timeRelated in this.Values ){
                iCalDateTime datetime = timeRelated as iCalDateTime;
                if( datetime != null && timezone != null  ){
                    datetime = timezone.ToUTC( datetime );

                    ret.Values.Add( datetime );
                } else {
                    ret.Values.Add( timeRelated );
                }
                
            }
            ret.TimeZoneId = null;

            return ret;
        }

        public override Object Clone()
        {
            iCalExceptionDateTimes ret = (iCalExceptionDateTimes)base.Clone();

            ret.Values = new List<iCalTimeRelatedType>();
            foreach( iCalTimeRelatedType timeRelated in this.Values ){
                ret.Values.Add( (iCalTimeRelatedType)timeRelated.Clone() );
            }
            
            ret.TimeZoneId = this.TimeZoneId;
            
            return ret;
        }
    }

    // 3.8.5.2
    public class iCalRecurrenceDateTimes  : iCalProperty
    {
        // rdate      = "RDATE" rdtparam ":" rdtval *("," rdtval) CRLF
        // rdtparam   = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" ("DATE-TIME" / "DATE" / "PERIOD")) /
        //            (";" tzidparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )
        // rdtval     = date-time / date / period
        // ;Value MUST match value type
        // tzidparam  = "TZID" "=" [tzidprefix] paramtext
        // tzidprefix = "/"
        public static String RepName = "rdate";

        // enum ValueType { DateTime, Date, Period }

        public List<iCalTimeRelatedType> Values =
            new List<iCalTimeRelatedType>();
        public String TimeZoneId = null;

        public iCalRecurrenceDateTimes( iCalComponent component )
            : base( component ) {}

        public iCalRecurrenceDateTimes( iCalLineContent content,
                                        iCalComponent component )
            : base( content, component )
        {
            String key;
            iCalParameter param;
            iCalDateTime datetime;

            iCalValueDataType.ValueType dataType = 0;
            key = iCalValueDataType.RepName;
            if( content.Params.ContainsKey( key ) ){
                param = content.Params[ key ];
                dataType = (iCalValueDataType.ValueType)
                    param.GetValueType( param.Value() );
            }

            String str = content.Value;
            String str1 = null;
            while( str.Length > 0 ){
                int index = str.IndexOf( ',' );
                if( index >= 0 ){
                    str1 = str.Substring( 0, index );
                    str = str.Substring( index + 1 );
                } else {
                    str1 = str;
                    str = "";
                }
                
                if( dataType == iCalValueDataType.ValueType.DateTime ){
                    this.Values.Add( new iCalDateTime( str1 ) );
                } else if( dataType == iCalValueDataType.ValueType.Date ){
                    this.Values.Add( new iCalDate( str1 ) );
                } else if( dataType == iCalValueDataType.ValueType.Period ){
                    this.Values.Add( new iCalPeriod( str1 ) );
                } else {
                    if( str1.IndexOf( "/" ) >= 0 ){
                        this.Values.Add( new iCalPeriod( str1 ));
                    } else {
                        datetime = new iCalDateTime( str1 );
                        if( datetime.Time == null ){
                            this.Values.Add( datetime.Date );
                        } else {
                            this.Values.Add( datetime );
                        }
                    }
                }
            }

            key = iCalTimeZoneIdentifierParameter.RepName;
            if( content.Params.ContainsKey( key ) ){
                iCalParameter timeZoneParam = content.Params[ key ];

                this.TimeZoneId = timeZoneParam.Value();
            }
        }

        public override Object Clone()
        {
            iCalRecurrenceDateTimes ret = (iCalRecurrenceDateTimes)base.Clone();

            ret.Values = new List<iCalTimeRelatedType>();
            foreach( iCalTimeRelatedType timeRelated in this.Values ){
                ret.Values.Add( (iCalTimeRelatedType)timeRelated.Clone() );
            }
            
            ret.TimeZoneId = this.TimeZoneId;
            
            return ret;
        }
    }

    // 3.8.5.3
    public class iCalRecurrenceRule : iCalProperty
    {
        // rrule      = "RRULE" rrulparam ":" recur CRLF
        // rrulparam  = *(";" other-param)

        public static String RepName = "rrule";

        public iCalRecurrence Value = null;

        public iCalRecurrenceRule( iCalComponent component )
            : base( component ) {}

        public iCalRecurrenceRule( iCalLineContent content,
                                   iCalComponent component )
            : base( content, component )
        {
            this.Value = new iCalRecurrence( content.Value );
        }            

        public override Object Clone()
        {
            iCalRecurrenceRule ret = (iCalRecurrenceRule)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalRecurrence)this.Value.Clone();
            }
            
            return ret;
        }
    }

    public class iCalRecurrenceRuleEnum : IEnumerator<iCalDateTimeStart>
    {
        iCalDateTimeStart Original;
        iCalRecurrenceRule RRule;
        IEnumerator<iCalTimeRelatedType> timeRelatedEnum;
        bool IsValid = false;
            
        Object IEnumerator.Current {
            get { return this.Current; }
        }
        
        iCalDateTimeStart IEnumerator<iCalDateTimeStart>.Current {
             get { return this.Current; }
        }

        public iCalDateTimeStart Current { get; set; }


        public iCalRecurrenceRuleEnum( iCalDateTimeStart original,
                                       iCalRecurrenceRule rrule )
        {
            this.Original = original;
            this.RRule = rrule;

            if( rrule != null && original != null ){
                this.timeRelatedEnum =
                    new iCalRecurrenceEnum( original.Value,
                                            rrule.Value );
                IsValid = true;
            }
        }

        public bool MoveNext()
        {
            if( this.IsValid && this.timeRelatedEnum.MoveNext() ){

                // check for until date of recurrence
                // <--
                iCalTimeRelatedType timeRelated = this.timeRelatedEnum.Current;
                iCalDateTime datetime = timeRelated as iCalDateTime;
                iCalDate date = timeRelated as iCalDate;

                if( datetime != null ){
                    iCalDateTime untilDateTime =
                        this.RRule.Value.Until as iCalDateTime;
                    if( untilDateTime != null ){
                        if( datetime.Time.IsUTC != untilDateTime.Time.IsUTC ){
                            iCalDateTimeStart dtstart =
                                (iCalDateTimeStart)this.Original.Clone();
                            dtstart.Value = datetime;
                            dtstart = (iCalDateTimeStart)dtstart.ToUTC();
                            iCalTimeRelatedType datetime2 = dtstart.Value;

                            dtstart = (iCalDateTimeStart)this.Original.Clone();
                            dtstart.Value = untilDateTime;
                            dtstart = (iCalDateTimeStart)dtstart.ToUTC();
                            iCalTimeRelatedType untilDateTime2 = dtstart.Value;
                            if( datetime2 > untilDateTime2 ){
                                return false;
                            }
                        } else {
                            if( datetime > untilDateTime ){
                                return false;
                            }
                        }
                    }
                } else if( date != null ){
                    iCalDate untilDate =
                        this.RRule.Value.Until as iCalDate;
                    if( untilDate != null ){
                        if( date > untilDate ){
                            return false;
                        }
                    }
                }
                // -->

                this.Current = (iCalDateTimeStart)this.Original.Clone();
                this.Current.Value = this.timeRelatedEnum.Current;

                return true;
            } else {
                return false;
            }
        }

        public void Reset(){
            if( this.IsValid ){
                this.timeRelatedEnum.Reset();
            }
        }

        public void Dispose(){
            if( this.IsValid ){
                this.timeRelatedEnum.Dispose();
            }
        }
    }

    // 3.8.6.1
    public class iCalAction : iCalProperty
    {
        // action      = "ACTION" actionparam ":" actionvalue CRLF
        // actionparam = *(";" other-param)
        // actionvalue = "AUDIO" / "DISPLAY" / "EMAIL"
        //             / iana-token / x-name
        public static String RepName = "action";
        public enum ValueType { Other=0, Audio, Display, Email }

        public ValueType Type = 0;
        public iCalText Value = null;

        public iCalAction( iCalComponent component ) : base( component ) {}

        public iCalAction( iCalLineContent content,
                           iCalComponent component )
            : base( content, component ) {

            this.Value = new iCalText( content.Value );
            this.Type = 0;

            String value = content.Value.ToLower();
            if( value == "audio" ){
                this.Type = ValueType.Audio;
            } else if( value == "display" ){
                this.Type = ValueType.Display;
            } else if( value == "email" ){
                this.Type = ValueType.Email;
            }
        }

        public override Object Clone()
        {
            iCalAction ret = (iCalAction)base.Clone();

            ret.Type = this.Type;
            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            
            return ret;
        }
    }

    // 3.8.6.2
    public class iCalRepeatCount : iCalProperty
    {
        // repeat  = "REPEAT" repparam ":" integer CRLF
        // ;Default is "0", zero.
        // repparam   = *(";" other-param)
        public static String RepName = "repeat";

        public int Value = 0;

        public iCalRepeatCount( iCalComponent component ) : base( component ) {}

        public iCalRepeatCount( iCalLineContent content,
                                iCalComponent component )
            : base( content, component ) {
            this.Value = Int32.Parse( content.Value );
        }

        public override Object Clone()
        {
            iCalRepeatCount ret = (iCalRepeatCount)base.Clone();

            ret.Value = this.Value;
            
            return ret;
        }
    }

    // 3.8.6.3
    public class iCalTrigger : iCalProperty
    {
        // trigger    = "TRIGGER" (trigrel / trigabs) CRLF
        // trigrel    = *(
        //            ;
        //            ; The following are OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" "DURATION") /
        //            (";" trigrelparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            ) ":"  dur-value
        // trigabs    = *(
        //            ;
        //            ; The following is REQUIRED,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" "VALUE" "=" "DATE-TIME") /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            ) ":" date-time
        public static String RepName = "trigger";

        public bool IsDuration = false;
        public iCalDurationDataType TriggerDuration = null;
        public iCalDateTime TriggerDateTime = null;

        public iCalTrigger( iCalComponent component ) : base( component ) {}

        public iCalTrigger( iCalLineContent content,
                            iCalComponent component )
            : base( content, component ) {

            String key = iCalValueDataType.RepName;
            if( content.Params.ContainsKey( key ) ){
                iCalParameter param = content.Params[ key ];
                iCalValueDataType.ValueType valueType =
                    (iCalValueDataType.ValueType)param.GetValueType( param.Value() );
                if( valueType == iCalValueDataType.ValueType.Duration ){
                    this.IsDuration = true;
                    this.TriggerDuration = new iCalDurationDataType( content.Value );
                    this.TriggerDateTime = null;
                } else {
                    this.IsDuration = false;
                    this.TriggerDuration = null;
                    this.TriggerDateTime = new iCalDateTime( content.Value );
                }
            } else {
                // this is syntax error, though
                this.IsDuration = true;
                this.TriggerDuration = new iCalDurationDataType( content.Value );
                this.TriggerDateTime = null;
            }
        }

        public override Object Clone()
        {
            iCalTrigger ret = (iCalTrigger)base.Clone();

            ret.IsDuration = this.IsDuration;
            if( this.TriggerDuration != null ){
                ret.TriggerDuration =
                    (iCalDurationDataType)this.TriggerDuration.Clone();
            }
            if( this.TriggerDateTime != null ){
                ret.TriggerDateTime =
                    (iCalDateTime)this.TriggerDateTime.Clone();
            }
            
            return ret;
        }        
    }

    // 3.8.7.1
    public class iCalDateTimeCreated : iCalProperty
    {
        // created    = "CREATED" creaparam ":" date-time CRLF
        // creaparam  = *(";" other-param)
        public static String RepName = "created";

        public iCalDateTime Value = null;

        public iCalDateTimeCreated( iCalComponent component )
            : base( component ) {}

        public iCalDateTimeCreated( iCalLineContent content,
                                    iCalComponent component )
            : base( content, component ) {

            this.Value = new iCalDateTime( content.Value );
        }

        public override Object Clone()
        {
            iCalDateTimeCreated ret = (iCalDateTimeCreated)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalDateTime)this.Value.Clone();
            }
            
            return ret;
        }
    }

    // 3.8.7.2
    public class iCalDateTimeStamp : iCalProperty
    {
        // dtstamp    = "DTSTAMP" stmparam ":" date-time CRLF
        // stmparam   = *(";" other-param)
        public static String RepName = "dtstamp";

        public iCalDateTime Value = null;

        public iCalDateTimeStamp( iCalComponent component )
            : base( component ) {}

        public iCalDateTimeStamp( iCalLineContent content,
                                  iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalDateTime( content.Value );
        }

        public override Object Clone()
        {
            iCalDateTimeStamp ret = (iCalDateTimeStamp)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalDateTime)this.Value.Clone();
            }
            
            return ret;
        }
    }

    // 3.8.7.3
    public class iCalLastModified : iCalProperty
    {
        // last-mod   = "LAST-MODIFIED" lstparam ":" date-time CRLF
        // lstparam   = *(";" other-param)
        public static String RepName = "last-modified";

        public iCalDateTime Value = null;

        public iCalLastModified( iCalComponent component )
            : base( component ) {}

        public iCalLastModified( iCalLineContent content,
                                 iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalDateTime( content.Value );
        }

        public override Object Clone()
        {
            iCalLastModified ret = (iCalLastModified)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalDateTime)this.Value.Clone();
            }
            
            return ret;
        }
    }

    // 3.8.3.4
    public class iCalSequenceNumber : iCalProperty
    {
        // seq = "SEQUENCE" seqparam ":" integer CRLF
        // ; Default is "0"
        // seqparam   = *(";" other-param)
        public static String RepName = "sequence";

        public int Value = 0;

        public iCalSequenceNumber( iCalComponent component )
            : base( component ) {}

        public iCalSequenceNumber( iCalLineContent content,
                                   iCalComponent component )
            : base( content, component ) {
            this.Value = Int32.Parse( content.Value );
        }

        public override Object Clone()
        {
            iCalSequenceNumber ret = (iCalSequenceNumber)base.Clone();

            ret.Value = this.Value;
            
            return ret;
        }

        public int CompareTo( iCalSequenceNumber other ){
            int otherseq = 0;
            if( other != null ){
                otherseq = other.Value;
            }
            return( this.Value - otherseq );
        }

        public static bool operator < ( iCalSequenceNumber date1,
                                        iCalSequenceNumber date2 ){
            if( ((Object)date1) == null ){
                if( ((Object)date2) != null ){
                    return true;
                } else {
                    return false;
                }
            } else {
                if( ((Object)date2) != null ){
                    int ret = date1.CompareTo( date2 );
                    if( ret < 0 ){
                        return true;
                    }
                } else {
                    return false;
                }
            }

            return false;
        }

        public static bool operator > ( iCalSequenceNumber date1,
                                        iCalSequenceNumber date2 ){
            if( ((Object)date1) == null ){
                if( ((Object)date2) != null ){
                    return false;
                } else {
                    return true;
                }
            } else {
                if( ((Object)date2) != null ){
                    int ret = date1.CompareTo( date2 );
                    if( ret > 0 ){
                        return true;
                    }
                } else {
                    return true;
                }
            }

            return false;
        }
        

    }

    // 3.8.8.3
    public class iCalRequestStatus : iCalProperty
    {
        // rstatus    = "REQUEST-STATUS" rstatparam ":"
        //              statcode ";" statdesc [";" extdata]

        // rstatparam = *(
        //            ;
        //            ; The following is OPTIONAL,
        //            ; but MUST NOT occur more than once.
        //            ;
        //            (";" languageparam) /
        //            ;
        //            ; The following is OPTIONAL,
        //            ; and MAY occur more than once.
        //            ;
        //            (";" other-param)
        //            ;
        //            )

        // statcode   = 1*DIGIT 1*2("." 1*DIGIT)
        // ;Hierarchical, numeric return status code

        // statdesc   = text
        // ;Textual status description

        // extdata    = text
        // ;Textual exception data.  For example, the offending property
        // ;name and value or complete property line.
        public static String RepName = "request-status";

        public iCalText Value = null;

        public iCalRequestStatus( iCalComponent component )
            : base( component ) {}

        public iCalRequestStatus( iCalLineContent content,
                                  iCalComponent component )
            : base( content, component ) {
            this.Value = new iCalText( content.Value );
        }

        public override Object Clone()
        {
            iCalRequestStatus ret = (iCalRequestStatus)base.Clone();

            if( this.Value != null ){
                ret.Value = (iCalText)this.Value.Clone();
            }
            
            return ret;
        }
    }
}