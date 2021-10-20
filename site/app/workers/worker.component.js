(function () {

    var app = angular.module('app');



    app.component('worker', {
        templateUrl: 'app/workers/worker.template.html',
        controller: WorkerController,
        bindings: {
            worker: '<',
            files: '<',
            childs: '<'

        }
    });

    function WorkerController(usersService, $scope, $state, sharedValues, filesService, $window) {

        this.sharedValues = sharedValues;
        this.scope = $scope;
        this.submit = _submit.bind(this);
        this.roles = usersService.roles;
        this.delete = _delete.bind(this);
        // this.selfEdit = angular.fromJson(localStorage.getItem('authorizationData')).userName == this.user.Email;
        this.role = localStorage.getItem('currentRole');

        this.uploadFile = _uploadFile.bind(this);
        this.uploadFileParud = _uploadFileParud.bind(this);
        this.uploadFileZikuyNeke = _uploadFileZikuyNeke.bind(this);
        this.uploadFileZikuyToshav = _uploadFileZikuyToshav.bind(this);
        this.uploadFileZikuyOleHadash = _uploadFileZikuyOleHadash.bind(this);
        this.uploadFileZikuyPsakDinMezonot = _uploadFileZikuyPsakDinMezonot.bind(this);
        this.uploadFileZikuyLuladMugbalut = _uploadFileZikuyLuladMugbalut.bind(this);
        this.uploadFileZikuyHayalEnd = _uploadFileZikuyHayalEnd.bind(this);
        this.uploadFileZikuyToarAkdemi = _uploadFileZikuyToarAkdemi.bind(this);

        this.uploadFileTiumMasAproove = _uploadFileTiumMasAproove.bind(this);
        this.uploadFileTiummas = _uploadFileTiummas.bind(this);
        this.uploadFileTiumMasTlush = _uploadFileTiumMasTlush.bind(this);
        this.clearSignuture = _clearSignuture.bind(this);


        this.removeFile = _removeFile.bind(this);
        this.init = _init.bind(this);

        this.removeChild = _removeChild.bind(this);
        this.addNewChild = _addNewChild.bind(this);
        this.saveWorker = _saveWorker.bind(this);
        this.getFileName = _getFileName.bind(this);


        this.uploadsUri = sharedValues.apiUrl + '/uploads/'
        this.foldertaz = "taz";

        this.cities = [];
        this.fileparud = "";

       // this.childs = [];

        this.ImageSignuture;

        this.init();
        function _init() {
           
            var obj = this.worker;

            
            Object.keys(obj).forEach(function (key, index) {

                if (key.indexOf("Date") != -1 && obj[key])
                {
                    
                    obj[key] = moment(obj[key]).startOf('day').toDate();

                }
              
            });
           

            usersService.getCitiesList().then(function (res) {

                this.cities = res; //["מעגלים", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua &amp; Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia &amp; Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Canada", "Cape Verde", "Cayman Islands", "Central Arfrican Republic", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cuba", "Curacao", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kosovo", "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauro", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "Norway", "Oman", "Pakistan", "Palau", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre &amp; Miquelon", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Korea", "South Sudan", "Spain", "Sri Lanka", "St Kitts &amp; Nevis", "St Lucia", "St Vincent", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad &amp; Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks &amp; Caicos", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States of America", "Uruguay", "Uzbekistan", "Vanuatu", "Vatican City", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe"];

                autocomplete(document.getElementById("txtCity"), this.cities);

            }.bind(this));


           
            //$('#bcPaint').bcPaint();


            this.tazfiles = this.getFileName(1);// this.files.filter(x => x.Type == 1)[0].FileName;
            this.fileparud = this.getFileName(2);
            this.filezikuyneke = this.getFileName(3);
            this.filezikuytoshav = this.getFileName(4);
            this.filezikuyolehadash = this.getFileName(5);
            this.filezikuypsakdinmezonot = this.getFileName(6);
            this.filezikuyluladmugbalut = this.getFileName(7);
            this.filezikuyhayalend = this.getFileName(8);

            this.filezikuytoarakdemi = this.getFileName(9);
            this.filetiummasaproove = this.getFileName(10);
            this.filetiummas = this.getFileName(11);

            this.filetiummastlush = this.getFileName(12);



            
            //var myCanvas = document.getElementById('bcPaintCanvas');
            //var ctx = myCanvas.getContext('2d');
            //var img = new Image;
            //img.onload = function () {
            //    ctx.drawImage(img, 0, 0); // Or at whatever offset you like
            //};
          
            //img.src = this.uploadsUri + "/" + this.worker.Id + "/Signature.png";

           

           




        }


        function _getFileName(type) {


            if (type == 1) {

                var rows = this.files.filter(x => x.Type == type);
                if (rows.length == 0) return [];
                else return rows;

            }


            var rows = this.files.filter(x => x.Type == type);
            if (rows.length == 0) return "";
            else
                return rows[0].FileName;
        }

        function _uploadFile(file) {
            this.tazfiles = this.tazfiles || [];
            var allfiles = file.split(",") || [];


            for (var i in allfiles) {
                var Obj = { "Id": 0, "Type": 1, "FileName": allfiles[i] };
                this.files.push(Obj);
                this.tazfiles.push(Obj);
            }
            // 
            //if (file) {
            //    var Obj = { "Id": 0, "HorseId": 22, "FileName": file };
            //    this.files.push(Obj);

            //}
        }

        function _uploadFileParud(file) {

            if (file) {

                var allCurrentFiles = this.files.filter(x => x.Type == 2);
                for (var i in allCurrentFiles) {
                    // if (this.files[i].FileName == file) {

                    allCurrentFiles.splice(i, 1);

                    // }
                }


                this.fileparud = file;
                var Obj = { "Id": 0, "Type": 2, "FileName": file };
                this.files.push(Obj);

            }

        }

        function _uploadFileZikuyNeke(file) {

            if (file) {

                this.filezikuyneke = file;
                this.files = this.files.filter(x => x.Type != 3);
                var Obj = { "Id": 0, "Type": 3, "FileName": file };
                this.files.push(Obj);

            }

        }

        function _uploadFileZikuyToshav(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 4);
                this.filezikuytoshav = file;
                var Obj = { "Id": 0, "Type": 4, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyOleHadash(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 5);
                this.filezikuyolehadash = file;
                var Obj = { "Id": 0, "Type": 5, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyPsakDinMezonot(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 6);
                this.filezikuypsakdinmezonot = file;
                var Obj = { "Id": 0, "Type": 6, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyLuladMugbalut(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 7);
                this.filezikuyluladmugbalut = file;
                var Obj = { "Id": 0, "Type": 7, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyHayalEnd(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 8);
                this.filezikuyhayalend = file;
                var Obj = { "Id": 0, "Type": 8, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileZikuyToarAkdemi(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 9);
                this.filezikuytoarakdemi = file;
                var Obj = { "Id": 0, "Type": 9, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileTiumMasAproove(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 10);
                this.filetiummasaproove = file;
                var Obj = { "Id": 0, "Type": 10, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileTiummas(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 11);
                this.filetiummas = file;
                var Obj = { "Id": 0, "Type": 11, "FileName": file };
                this.files.push(Obj);


            }

        }

        function _uploadFileTiumMasTlush(file) {

            if (file) {
                this.files = this.files.filter(x => x.Type != 12);
                this.filetiummastlush = file;
                var Obj = { "Id": 0, "Type": 12, "FileName": file };
                this.files.push(Obj);


            }

        }



        function _clearSignuture() {

            //var canvas = document.getElementById("bcPaintCanvas");
            //var context = canvas.getContext('2d');
            //context.clearRect(0, 0, canvas.width, canvas.height); //clear html5 canvas

           // $('#bcPaintCanvas').html('');
          //  $.fn.bcPaint.clearCanvas();


            $scope.clear();

          //  $.fn.bcPaint.export();
        }



        function _addNewChild() {

            if (!this.newChild || !this.newChild.Name || !this.newChild.BirthDate) {

                alertMessage("יש למלא שדות תיקניים!");
                return;
            }

            if (!this.newChild.Taz || ValidateID(this.newChild.Taz) != 1) {

                alertMessage("שדה תעודת זהות אינו תקין!");
                return;
            }






            this.childs = this.childs || [];
            this.childs.push({ Id: this.newChild.Id, Name: this.newChild.Name, Taz: this.newChild.Taz, BirthDate: this.newChild.BirthDate, IsInHouse: this.newChild.IsInHouse, IsBituaLeumi: this.newChild.IsBituaLeumi });


            this.newChild.Name = "";
            this.newChild.Taz = "";
            this.newChild.BirthDate = "";
            this.newChild.IsInHouse = false;
            this.newChild.IsBituaLeumi = false;


            // this.initNewHorse();
        }

        function _removeChild(child) {
            
            for (var i in this.childs) {
                if (this.childs[i].Id == child.Id) {
                    this.childs.splice(i, 1);
                }
            }
        }


        function ValidateID(str) {

            // DEFINE RETURN VALUES
            var R_ELEGAL_INPUT = -1;
            var R_NOT_VALID = -2;
            var R_VALID = 1;

            //INPUT VALIDATION

            // Just in case -> convert to string
            var IDnum = str;

            // Validate correct input
            if ((IDnum.length > 9) || (IDnum.length < 5))
                return R_ELEGAL_INPUT;
            if (isNaN(IDnum))
                return R_ELEGAL_INPUT;

            // The number is too short - add leading 0000
            if (IDnum.length < 9) {
                while (IDnum.length < 9) {
                    IDnum = '0' + IDnum;
                }
            }

            // CHECK THE ID NUMBER
            var mone = 0, incNum;
            for (var i = 0; i < 9; i++) {
                incNum = Number(IDnum.charAt(i));
                incNum *= (i % 2) + 1;
                if (incNum > 9)
                    incNum -= 9;
                mone += incNum;
            }
            if (mone % 10 == 0)
                return R_VALID;
            else
                return R_NOT_VALID;
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

        function _saveWorker(type) {

            var obj = angular.copy(this.worker);
            Object.keys(obj).forEach(function (key, index) {

                if (key.indexOf("Date") != -1 && obj[key]) {

                    
                    obj[key].setHours((obj[key]).getHours() + 3);
              

                }
                
            });

            if (type == 1) {
               //$.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע שומרים את הנתונים  <br/>אנא המתנ/י...</div></h5>' });
               
                var Signature = $scope.accept();
                if (!Signature.isEmpty) {
                    obj["ImgData"] = Signature.dataUrl;
                } else {
                    obj["ImgData"] = "";

                }

                //var paintCanvas = document.getElementById('bcPaintCanvas');
                //var imgData = paintCanvas.toDataURL('image/png');
                //obj["ImgData"] = imgData;

                //// אם זה ריק אל תשלח כלום
                //if (imgData.indexOf("ECBAgQIEDgAQNaAJ+CzbUNAAAAAElFTkSuQmCC") != -1)
                //    obj["ImgData"] = "";
              
                usersService.updateWorker(obj, this.files,this.childs,type).then(function (worker) {
                  //  this.worker = worker;
                   alertMessage('הנתונים נשמרו בהצלחה!');
                   // $.unblockUI();
                   //
                }.bind(this));

            }

            if (type == 2) {
                if (this.scope.workerForm.$valid) {
                    $.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע מעבדים את הנתונים ומייצרים קובץ  PDF ושולחים אותו למשרד <br/>אנא המתנ/י...</div></h5>' });



                    var Signature = $scope.accept();
                    if (!Signature.isEmpty) {
                        obj["ImgData"] = Signature.dataUrl;
                    } else {
                        obj["ImgData"] = "";

                    }


                    usersService.updateWorker(obj, this.files, this.childs, type).then(function (worker) {

                      

                        $.unblockUI();
                        if (worker.Status =="נשלח למשרד")
                            alertMessage('הנתונים נשלחו למשרד בהצלחה!');
                        else
                            alertMessage(worker.Status);
                       
                    }.bind(this));
                }

                else {
                    alertMessage("יש למלא את כל השדות המסומנים באדום , אלו שדות חובה");

                }


            }
         
            if (type == 3) {
                $.blockUI({ css: {}, message: '<h5><div id="loader"></div><div class="tzahiStyle"> אנחנו כרגע מעבדים את הנתונים ומייצרים קובץ PDF  <br/>אנא המתנ/י...</div></h5>' });

                var Signature = $scope.accept();
                if (!Signature.isEmpty) {
                    obj["ImgData"] = Signature.dataUrl;
                } else {
                    obj["ImgData"] = "";

                }


              
                usersService.updateWorker(obj, this.files, this.childs, type).then(function (worker) {
                   
                    $.unblockUI();
                    $window.open(this.uploadsUri + "/" + this.worker.Id + "/OfekAllPdf.pdf", '_blank');
                    
                }.bind(this));

            }


        }

        function _submit() {

        }

        function _delete() {

            var dd = this.worker;
            if (confirm('האם למחוק את המשתמש?')) {
                usersService.deleteUser(this.user.Id).then(function (res) {
                    $state.go('workers');
                });
            }
        }


    }



})();