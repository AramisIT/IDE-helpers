﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AramisIDE.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AramisIDE.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap brightness {
            get {
                object obj = ResourceManager.GetObject("brightness", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using Aramis.Attributes;
        ///using Aramis.DatabaseConnector;
        ///using Aramis.Enums;
        ///using Aramis.Core;
        ///using Aramis.SystemConfigurations;
        ///using Catalogs;
        ///
        ///namespace Catalogs
        ///    {{    
        ///    [Catalog(Description = &quot;&quot;, FieldCaptionInUI = &quot;&quot;, GUID = &quot;{0}&quot;, DescriptionSize = 35, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false, ShowCreationDate = true, ShowLastModifiedDate = true)]
        ///    public interface {1} : ICatalog
        ///        {{
        ///        [DataField(Description = &quot;Настройки&quot;)]	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CatalogTemplate {
            get {
                return ResourceManager.GetString("CatalogTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using Aramis.Attributes;
        ///using Aramis.DatabaseConnector;
        ///using Aramis.Enums;
        ///using Aramis.Core;
        ///using Aramis.SystemConfigurations;
        ///using Catalogs;
        ///
        ///namespace Documents
        ///    {{    
        ///    [Document(Description = &quot;&quot;, GUID = &quot;{0}&quot;)]
        ///    public interface {1} : IDocument
        ///        {{    
        ///		Table&lt;IExsTable&gt; ExsTable {{ get; }}  
        ///        }}        
        ///
        ///    public interface IExsTable : ITableRow
        ///        {{
        ///        [DataField(Description = &quot;&quot;)]
        ///        Ref&lt;IExsCatalog&gt; ExsField {{ get; set; } [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DocumentTemplate {
            get {
                return ResourceManager.GetString("DocumentTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon ProgramIcon {
            get {
                object obj = ResourceManager.GetObject("ProgramIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to use master;
        ///
        ///ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE 
        ///
        ///ALTER DATABASE [{0}] SET SINGLE_USER WITH NO_WAIT
        ///
        ///ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE; 
        ///
        ///WAITFOR DELAY &apos;00:00:01&apos;
        ///
        ///GO
        ///
        ///ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE 
        ///
        ///ALTER DATABASE [{0}] SET SINGLE_USER WITH NO_WAIT
        ///
        ///ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE; 
        ///
        ///print &apos;restoring&apos;
        ///
        ///RESTORE DATABASE [{0}]
        ///   FROM DISK = &apos;{1}&apos;
        ///
        ///GO
        ///
        ///USE [{0}]
        ///
        ///GO
        ///
        ///IF  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RestoreSqlCommand {
            get {
                return ResourceManager.GetString("RestoreSqlCommand", resourceCulture);
            }
        }
    }
}
