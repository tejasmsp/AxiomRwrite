using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxiomAutomation
{
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
}
