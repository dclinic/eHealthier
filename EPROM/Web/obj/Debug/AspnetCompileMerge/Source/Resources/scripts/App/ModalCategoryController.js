app.controller('ModalCategoryController', ['$scope', '$rootScope', '$http', '$uibModalInstance', function ($scope, $rootScope, $http, $uibModalInstance) {

    $scope.ModalCategory = {
        issubmitted: false,
        IsLoading: false,
        Initialize: function () {
        },
        Methods: {
            Create: function (from) {
                $scope.ModalCategory.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.ModalCategory.IsLoading = true;
                    $scope.ModalCategory.Services.Create({
                        CategoryName: $scope.ModalCategory.CategoryName,
                        IsActive: $scope.ModalCategory.IsActive
                    });
                }
            }
        },
        Services: {
            Create: function (data, from) {
                $http.post('/api/category/Post', data).success(function (response) {
                    $scope.ModalCategory.IsLoading = false;
                    $rootScope.SelectedCategory = response;
                    $uibModalInstance.close();
                });
            }
        },
        UI: {
            ImportCategoryModal: {
                Ok: function () {
                    $uibModalInstance.close();
                },

                Cancel: function () {
                    $uibModalInstance.dismiss('cancel');
                },
            },
        }
    };

    $scope.ModalCategory.Initialize();
}]);