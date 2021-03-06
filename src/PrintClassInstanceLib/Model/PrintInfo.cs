using System;
using System.Collections.Generic;
using System.Reflection;

namespace PrintClassInstanceLib.Model
{
    public class PrintInfo
    { 
        public string Name { get; set; }
        public List<PrintInfo> Values { get; set; }=new List<PrintInfo>();
        public string Type { get; set; }
        public object Value { get; set; }
        public string Namespace { get; set; }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public bool IsEnum { get; set; }

        public object RawMemberValue { get; set; }
        public Type RawMemberType { get; set; }
        public Func<MemberInfo, object, object,string> SetVal { get; set; }
        public MemberInfo Member { get; set; }
    }
}