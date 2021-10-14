(function () {

    var app = angular.module('app');



    app.component('docs', {
        templateUrl: 'app/docs/docs.template.html',
        controller: DocsController,
        bindings: {
            worker: '<',
            files: '<',
            childs: '<'

        }
    });

    function DocsController(usersService, $scope, $state, sharedValues, filesService, $window) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        //this.submit = _submit.bind(this);
        this.roles = usersService.roles;
      
        this.role = localStorage.getItem('currentRole');

        this.uploadFile = _uploadFile.bind(this);
        this.removeFile = _removeFile.bind(this);

       

        function _uploadFile(file) {
            this.tazfiles = this.tazfiles || [];
            this.files = this.files || [];
            var allfiles = file.split(",") || [];

           

            for (var i in allfiles) {
                var Obj = { "Id": 0, "Type": 1, "FileName": allfiles[i] };
                this.files.push(Obj);
                this.tazfiles.push(Obj);
            }
           
        }

        function _removeFile(file, type) {
            filesService.delete(file).then(function () {
                if (type == 1)
                    for (var i in this.files) {
                        if (this.files[i].FileName == file) {

                            this.files.splice(i, 1);
                            this.tazfiles.splice(i, 1);
                            break;
                        }
                    }




            }.bind(this));
        }

    }



})();