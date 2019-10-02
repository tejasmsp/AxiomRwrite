app.service('ProposalFeesService', function ($http, configurationService) {
    var ProposalFeesService = [];

    ProposalFeesService.GetProposalFees = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetProposalFees?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };

    ProposalFeesService.GetProposalFeesCalculation = function (OrderId, PartNo, RecordTypeId, PageNo) {
        return $http.get(configurationService.basePath + "GetProposalFeesCalculation?OrderId=" + OrderId + "&PartNo=" + PartNo + "&RecordTypeId=" + RecordTypeId + "&PageNo=" + PageNo);
    }
    ProposalFeesService.VerifyProposalFees = function (obj) {
        return $http.post(configurationService.basePath + "VerifyProposalFees", obj);
    }
    ProposalFeesService.SaveProposalFees = function (objSave) {
        return $http.post(configurationService.basePath + "SaveProposalFees", objSave);
    }
    return ProposalFeesService;
});