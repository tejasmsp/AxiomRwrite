app.factory('notificationFactory', function (localStorageMesseage) {
    /// <summary>
    /// s the specified local storage messeage.
    /// </summary>
    /// <param name="localStorageMesseage">The local storage messeage.</param>
    /// <returns></returns>
    return {
        customSuccess: function (text) {
            toastr.success(text);
        },
        successAdd: function (text) {
            toastr.success(text, "Added Successfully");
        },
        successSave: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0013"));
        },
        successEdit: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0026"));
        },
        successDelete: function () {
            toastr.success("Deleted Successfully");
        },
        errorSave: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0010"));
        },
        errorEdit: function (text) {
            toastr.error(text, "Error in Edit");
        },
        errorDelete: function (text) {
            toastr.error(text, "Error in Delete");
        },
        success: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0025"));
        },
        error: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0019"));
        },
        customError: function (text) {
            toastr.error(text);
        },
        FKReferenceDelete: function (text) {
            toastr.error(text, "The Delete Statement Conflict With REFERENCE constraint.The Statement has been terminated.");
        },
        recordInactiveSuccess: function () {
            toastr.success("Record Inactivated Successfully");
        },

        //////


        EmployeeNotFound: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0001"));
        },

        NoChangesMadeSaveSkipped: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0002"));
        },
        PerformenceReviewNotYetAvail: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0003"));
        },

        ReviewNotFound: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0004"));
        },

        ReviewNotFoundForReviewId: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0005"));
        },
        StatusChangeToCreated: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0006"));
        },
        StatusEmpSubRevChangeToSave: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0007"));
        },
        StatusSuperviserSubRevChangeToSave: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0008"));
        },

        UnableToDisplayReviewdueErrorBelow: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0009"));
        },
        UnableToSaveCareerprogchangedueErrBelow: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0010"));
        },

        UnableToSaveYourChangedueErrBelow: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0011"));
        },


        YourChangeHaveBeenCancelled: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0012"));
        },

        YourChangeHaveBeenSaved: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0013"));
        },

        YourManagerHasNotStartedYourReview: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0014"));
        },


        YoursupervisorHasyetReleaseThisInfo: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0015"));
        },

        NoDataIsOnFile: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0016"));
        },


        AnUnexpectedErrorHasOccured: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0019"));
        },

        YouMustSelectScoreforeachCompetency: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0017"));
        },


        UnableToSave: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0018"));
        },



        AccessDenied: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0020"));
        },


        ThisSectionOfreviewisNotReadyForYouToSee: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0021"));
        },


        BolItalicFormattingNote: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0022"));
        },


        TextMustNotExceed2000Char: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0023"));
        },


        UpdateSuccessfull: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0024"));
        },


        AllChangeSavedSuccessfully: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0025"));
        },


        GeneralSectionInformationSaved: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0026"));
        },


        UpdateNotAllowedAskSupervisorToReview: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0027"));
        },


        DevelopementPlaneInfoSaved: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0028"));
        },


        CarrerProgressionInfoSaved: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0029"));
        },


        PleaseSelectCorporateGoal: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0030"));
        },


        PleaseSelectObjectivePriority: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0031"));
        },


        NoEditRightAssigned: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0032"));
        },


        DoYouWantToDelete: function () {
            
            return localStorageMesseage.FilterWithresourceKey("M0033");
        },


        PleaseSupplyTheObjectiveName: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0034"));
        },


        TheObjectiveMust60CharOrLess: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0035"));
        },


        PleaseSupplyObjectiveDiscription: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0036"));
        },


        ObjectiveDiscriptionMust2000CharOrLess: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0037"));
        },

        successCommentEdit: function () {
            toastr.success("successfully Updated");
        },

        ReviewStatusChangedToAccepted: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0038"));
        },


        AsReviewStatusChangedToAcceptedResubmitreviewForAgreement: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0039"));
        },


        PerformrnceReviewNotFoundforYearof: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0040"));
        },


        ObjectiveReviewNotFoundForYearOf: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0041"));
        },


        ObjectivesForTheYearOf: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0042"));
        },


        NoObjectivesAreReadyForYouToSee: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0043"));
        },


        PleaseEnterTheLastNameAndPressGreenButton: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0044"));
        },


        UnableToSendTheEmail: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0045"));
        },


        PleaseSelectAcknowledgementOption: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0046"));
        },


        TheSupervisorEntriesForCompetencySectionIsIncomplete: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0047"));
        },


        TheSupervisorEntriesForEmpDevSectionIsIncomplete: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0048"));
        },


        TheEmployeeEntriesForOverallRatingSectionIsIncomplete: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0049"));
        },


        TheEmployeeEntriesForSelfAssesmentSectionIsIncomplete: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0051"));
        },


        TheEmployeeEntriesForDevelopementSectionIsIncomplete: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0052"));
        },



        EditThisObjective: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0053"));
        },


        ReviewIsNotOnFile: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0054"));
        },


        FinalAssessmentReviewEntryForReviewIsNotOnFile: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0055"));
        },


        ObjectivesReviewEntryForReviewIsNotOnFile: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0056"));
        },


        UnableToFindApprovarRecordForCurrentUser: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0057"));
        },


        ReviewRecordCreated: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0058"));
        },


        StatusOfEmployeeSubreviewChangedToCompleted: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0059"));
        },


        StatusOfSupervisorSubreviewChangedToCompleted: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0060"));
        },


        ReOpenedDataEntriesForEmployee: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0061"));
        },


        ReOpenedDataEntriesForSuperVisor: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0062"));
        },

        ReviewStateSetToWaitingForApproval: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0063"));
        },


        SupervisorApprovalRecieved: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0064"));
        },

        StatusChangeToApproved: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0065"));
        },

        StatusChangeToSentToEmployee: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0066"));
        },

        EmployeeRequestedSecondLevelManagerReview: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0067"));
        },

        ReviewStateSetToAcknowlwdge: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0068"));
        },

        PleaseSelectReopenOption: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0069"));
        },

        PerformanceReviewYetNotAvailable: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0070"));
        },

        SetOfObjectivesWereCreatedButNeverApprovedThroughFormalWorkFlow: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0071"));
        },

        DoubleClickOnTitleBarToMaximize: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0072"));
        },

        TheSupervisorNotYetReleaseThisInformation: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0073"));
        },

        YourRequestToBeIncludeInTheReviewHasBeenRemoved: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0074"));
        },

        YourApprovalWillNowBeRequired: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0075"));
        },

        CommentMustNotExceed2000Char: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0076"));
        },

        NotAllCompetenciesHaveBeenRated: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0077"));
        },

        CurrentLengh: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0078"));
        },

        IsRequired: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0079"));
        },

        DateMustBeInFuture: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0080"));
        },

        WouldYouLikeToSaveChangesBeforeLeaving: function () {
            return localStorageMesseage.FilterWithresourceKey("M0081");
        },
        DataEntryHasBeenLocked: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0082"));
        },
        YourDataHasBeenSavedAndSubmitted: function () {
            toastr.success(localStorageMesseage.FilterWithresourceKey("M0083"));
        },

        YourDataHasBeenSavedAndErrorOccuredWhileSubmitted: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0084"));
        },

        SupervisorEntriesForTheAnnualObjectivesSectionAreIncomplete: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0085"));
        },
        Value1MustNotExceedValue2Character: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0090"));
        },
        UnableToRunReportYourEmployeeRecordDoesNotHaveValidEmailAddress: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0091"));
        },

        YourReportRequestHasBeenSubmittedWillProcessedShortly: function () {
            toastr.error(localStorageMesseage.FilterWithresourceKey("M0092"));
        },
        InvalidImageType : function () {
            toastr.error("Acceptable file types: bmp, jpg, png, doc, docx, pdf");
        },
        InvalidDocumentType: function () {
            toastr.error("Acceptable file types: doc, docx, pdf");
        },
        InvalidExcelFileType: function () {
            toastr.error("Acceptable file types: xlsx, xls");
        },
        ExcelFileDataImported: function () {
            toastr.success("Excel data has been imported successfully in the fields.");
        },        
        NoChangesHaveBeenMade: function () {
            toastr.warning("No Changes have been made.");
        }

    };
});

app.service('localStorageMesseage', function ($filter, localStorageService) {


    this.FilterWithresourceKey = function (resourceKey) {

        var notifications = localStorageService.get("NotificationMesseges");

        if (notifications != null) {

            var filterAvail = $filter('filter')(notifications.responseJSON, { ResourceKey: resourceKey }, true);
            if(filterAvail.length)
            {
                return filterAvail[0]["ResourceValue"];
            }
        }
        else {
            return 'Notification Messeage Not Found';
        }
        return "";
    }
});

app.factory('httpInterceptor', ['$q', '$rootScope',
    function ($q, $rootScope) {
        var loadingCount = 0;

        return {
            request: function (config) {
                if (++loadingCount === 1) $rootScope.$broadcast('loading:progress');
                return config || $q.when(config);
            },

            response: function (response) {
                if (--loadingCount === 0) $rootScope.$broadcast('loading:finish');
                return response || $q.when(response);
            },

            responseError: function (response) {
                if (--loadingCount === 0) $rootScope.$broadcast('loading:finish');
                return $q.reject(response);
            }
        };
    }
]).config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('httpInterceptor');
}]);

