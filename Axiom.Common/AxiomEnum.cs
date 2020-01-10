using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Axiom.Common
{
    public class AxiomEnum
    {
    }

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
        Unknown = 0
        , Common = 1
        , Confirmation = 2
        , FaceSheet = 3
        , StatusLetters = 4
        , Waiver = 5
        , AttorneyOfRecords = 6
        , Interrogatories = 7
        , TargetSheets = 8
        , StatusProgressReports = 9
        , CerticicationNOD = 10
        , CollectionLetters = 11
        , Notices = 12
        , AttorneyForms = 13
        , FaceSheetNew = 14
    }

    public enum SendRequestType
    {
        FaxNumber = 0
            , Mail_StandardFolder = 1
            , Email = 2
            , Upload = 3
            , ProcessServer = 4
            , CertifiedMail = 5
    }
    public enum FileType
    {
        Authorization_Blank = 1,
        Authorization = 2,
        Request = 3,
        Other = 18
    }
    public enum OperationInitiatedFrom
    {
        AutomationService = 1,
        QuickFormService = 2,
        WebForm = 3
    }
}
