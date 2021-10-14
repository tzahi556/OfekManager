(function () {

    var app = angular.module('app');

  
    app.component('workers', {
        templateUrl: 'app/workers/workers.template.html',
        controller: WorkersController,
        bindings: {
            workers: '<'
            
        }
    });

    function WorkersController(usersService, sharedValues, $state) {

   
        this.roles = usersService.roles;
        this.sharedValues = sharedValues;
        this.getHebRole = _getHebRole.bind(this);

        this.delete = _delete.bind(this);
        this.uploadsUri = sharedValues.apiUrl + '/uploads/';
        function _getHebRole(id) {

        
            return this.sharedValues.roles.filter(x => x.id == id)[0].name;//(users[i].Role);
        }


        function _delete(workerid) {
            if (confirm('האם למחוק את העובד?')) {
              
                var ctrl = this;
                usersService.deleteWorker(workerid).then(function (res) {

                   
                    ctrl.workers = res;
                   // $state.go('workers');
                    //ctrl.user.Deleted = true;
                    //$state.go('students');
                });
            }
        }
    }

})();