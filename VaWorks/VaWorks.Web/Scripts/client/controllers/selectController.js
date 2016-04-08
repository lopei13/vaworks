'use strict';
app.controller('selectController',
    ['$scope', '$http',
    function ($scope, $http) {

        $scope.loading = true;

        $scope.organizationId;
        $scope.valves;
        $scope.valvemodels;
        $scope.valvesizes;
        $scope.actuators;
        $scope.actuatormodels;
        $scope.actuatorsizes;
        $scope.materials;
        $scope.options;
        $scope.kit;
        $scope.selectedValve;
        $scope.selectedValveModel;
        $scope.selectedValveSize;
        $scope.selectedActuator;
        $scope.selectedActuatorModel;
        $scope.selectedActuatorSize;
        $scope.selectedMaterial;
        $scope.selectedOption;
        $scope.hasValves = false;
        $scope.hasValveModels = false;
        $scope.hasValveSizes = false;
        $scope.hasActuators = false;
        $scope.hasActuatorModels = false;
        $scope.hasActuatorSizes = false;
        $scope.hasMaterials = false;
        $scope.hasOptions = false;
        $scope.hasKit = false;
        $scope.currentStep = 1;
        $scope.hasSearchResults = true;

        $scope.page = function (page) {
            if (page == 4) {
                if (!$scope.hasKit) {
                    page -= 1;
                }
            }

            if (page == 3) {
                if (!$scope.hasOptions) {
                    page -= 1;
                }
            }

            if (page == 2) {
                if (!$scope.hasMaterials) {
                    page -= 1;
                }
            }

            $scope.currentStep = page;
        };

        $http({
            method: "GET",
            url: "/KitSelection/GetValveManufacturers",
            params: { mfg: $scope.selectedValve, organizationId: $scope.organizationId }
        }).then(function success(res){
            $scope.valves = res.data;
            $scope.hasValves = $scope.valves.length > 0;
            $scope.hasValveModels = false;
            $scope.hasValveSizes = false;
            $scope.hasActuators = false;
            $scope.hasActuatorModels = false;
            $scope.hasActuatorSizes = false;
            $scope.hasMaterials = false;
            $scope.hasOptions = false;
            $scope.page(1);
            $scope.$emit('DONE');
        }, function error(res){
            alert('No valves found.');
        }).finally(function () {
            $scope.loading = false;
        });

        $scope.getValveModels = function () {
            $scope.loading = true;
            $scope.hasValveSizes = false;
            $scope.hasActuators = false;
            $scope.hasActuatorModels = false;
            $scope.hasActuatorSizes = false;
            $scope.hasMaterials = false;
            $scope.hasOptions = false;
            $scope.selectedValveModel = "";
            $scope.selectedValveSize = "";
            $scope.selectedActuator = "";
            $scope.selectedActuatorModel = "";
            $scope.selectedActuatorSize = "";
            $scope.selectedMaterial = "";
            $scope.selectedOption = "";
            $http({
                method: "GET",
                url: "/KitSelection/GetValveModels",
                params: { mfg: $scope.selectedValve, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.valvemodels = res.data;
                $scope.hasValveModels = $scope.valvemodels.length > 0;

            }, function error(res) {
                alert('No valves found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getValveSizes = function () {
            $scope.loading = true;
            $scope.hasActuators = false;
            $scope.hasActuatorModels = false;
            $scope.hasActuatorSizes = false;
            $scope.hasMaterials = false;
            $scope.hasOptions = false;
            $scope.selectedValveSize = "";
            $scope.selectedActuator = "";
            $scope.selectedActuatorModel = "";
            $scope.selectedActuatorSize = "";
            $scope.selectedMaterial = "";
            $scope.selectedOption = "";
            $http({
                method: "GET",
                url: "/KitSelection/GetValveSizes",
                params: { mfg: $scope.selectedValve, model: $scope.selectedValveModel, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.valvesizes = res.data;
                $scope.hasValveSizes = $scope.valvesizes.length > 0;
            }, function error(res) {
                alert('No valves found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuators = function () {
            $scope.loading = true;
            $scope.hasActuatorModels = false;
            $scope.hasActuatorSizes = false;
            $scope.hasMaterials = false;
            $scope.hasOptions = false;
            $scope.selectedActuator = "";
            $scope.selectedActuatorModel = "";
            $scope.selectedActuatorSize = "";
            $scope.selectedMaterial = "";
            $scope.selectedOption = "";
            $http({
                method: "GET",
                url: "/KitSelection/GetActuators",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.actuators = res.data;
                $scope.hasActuators = $scope.actuators.length > 0;
            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuatorModels = function () {
            $scope.loading = true;
            $scope.hasActuatorSizes = false;
            $scope.hasMaterials = false;
            $scope.hasOptions = false;
            $scope.selectedActuatorModel = "";
            $scope.selectedActuatorSize = "";
            $scope.selectedMaterial = "";
            $scope.selectedOption = "";
            $http({
                method: "GET",
                url: "/KitSelection/GetActuatorModels",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, mfg: $scope.selectedActuator, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.actuatormodels = res.data;
                $scope.hasActuatorModels = $scope.actuatormodels.length > 0;

            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuatorSizes = function () {
            $scope.loading = true;
            $scope.hasMaterials = false;
            $scope.hasOptions = false;
            $scope.selectedActuatorSize = "";
            $scope.selectedMaterial = "";
            $scope.selectedOption = "";
            $http({
                method: "GET",
                url: "/KitSelection/GetActuatorSizes",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, mfg: $scope.selectedActuator, model: $scope.selectedActuatorModel, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.actuatorsizes = res.data;
                $scope.hasActuatorSizes = $scope.actuatorsizes.length > 0;

            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getMaterials = function () {
            $scope.loading = true;
            $scope.hasOptions = false;
            $scope.selectedMaterial = "";
            $scope.selectedOption = "";
            $http({
                method: "GET",
                url: "/KitSelection/GetKitMaterials",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, actuatorInterface: $scope.selectedActuatorSize.InterfaceCode, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.materials = res.data;
                $scope.hasMaterials = $scope.materials.length > 0;
                $scope.selectedMaterial = "";
                $scope.selectedOption = "";
                $scope.page(2);
            }, function error(res) {
                alert('No materials found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };


        $scope.getOptions = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetKitOptions",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, actuatorInterface: $scope.selectedActuatorSize.InterfaceCode, materialId: $scope.selectedMaterial.KitMaterialId, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.options = res.data;
                $scope.hasOptions = $scope.options.length > 0;
                $scope.selectedOption = "";
                $scope.page(3);
            }, function error(res) {
                alert('No options found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getKit = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetKit",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, actuatorInterface: $scope.selectedActuatorSize.InterfaceCode, materialId: $scope.selectedMaterial.KitMaterialId, optionId: $scope.selectedOption.KitOptionId, organizationId: $scope.organizationId }
            }).then(function success(res) {
                if (res.data.length > 0) {
                    $scope.kit = res.data[0];
                    $scope.hasKit = true;
                    $scope.page(4);
                }
            }, function error(res) {
                alert('No kit found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };
    }]);