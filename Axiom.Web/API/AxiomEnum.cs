using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Axiom.Web.API
{
    public class AxiomEnum { }

    public static class Extensions
    {
        public static string GetDescription(this Enum e)
        {
            var attribute =
                e.GetType()
                    .GetTypeInfo()
                    .GetMember(e.ToString())
                    .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? e.ToString();
        }
    }
    public enum QueryType
    {
        Unknown,
        Common,
        Confirmation,
        FaceSheet,
        StatusLetters,
        Waiver,
        AttorneyOfRecords,
        Interrogatories,
        TargetSheets,
        StatusProgressReports,
        CerticicationNOD,
        CollectionLetters,
        Notices,
        AttorneyForms,
        FacesheetNew

    }
    public enum FileType
    {
        Authorization_Blank = 1,
        Authorization = 2,
        Request = 3,
        Other = 18

    }
}