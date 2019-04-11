var app = new Vue({
    el: '#app',
    data: {
        scanner: null,
        activeCameraId: null,
        cameras: [],
        scans: []
    },
    mounted: function () {
        var self = this;
        self.scanner = new Instascan.Scanner({ video: document.getElementById('preview'), scanPeriod: 5 });
        self.scanner.addListener('scan', function (content, image) {

            var audio = new Audio('../../audio/beep.mp3');
            audio.play();


            $.ajax({
                type: "GET",
                async: false,
                url: content,
                dataType: "json",
                success: function (data) {
                    if (data.found == 1) {
                        var audio = new Audio('../../audio/beep.mp3');
                        audio.play();

                        var now = new Date();
                        var n = now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();


                        if (data.InOrOut == 1) {

                            var welcomeMsg = "Welcome " + data.Fullname;
                            var imagePath = '<br><centre><img style="padding-left:0%;" src="../../Membership/' + data.PlaceId + '/' + data.PicturePath + '"></centre>';

                            $.confirm({
                                title: welcomeMsg,
                                content: '<form class="formName">' +
                                '<div class="form-group">' +
                                '<label>Time In</label> <br><input type="text" class="form-control" value="' + n + '" />' +
                                '</div>' +
                                '</form>' + imagePath,
                                type: 'green',
                                animation: 'zoom',
                                animationClose: 'top',
                                buttons: {
                                    cancel: {
                                        text: "OK",
                                        btnClass: 'btn-success',
                                        keys: ['enter'],
                                        action: function ()
                                        {
                                            window.location.href = "/fitness/Index/" + data.PlaceId;
                                        }
                                    }
                                },
                                onContentReady: function () {
                                }
                            });
                        }
                        else {
                            var welcomeMsg = "Goodbye " + data.Fullname;
                            var imagePath = '<br><centre><img style="padding-left:0%;" src="../../Membership/' + data.PlaceId + '/' + data.PicturePath + '"></centre>';

                            $.confirm({
                                title: welcomeMsg,
                                content: '<form class="formName">' +
                                '<div class="form-group">' +
                                '<label>Time Out</label> <br><input type="text" class="form-control" value="' + n + '" />' +
                                '</div>' +
                                '</form>' + imagePath,
                                type: 'green',
                                animation: 'zoom',
                                animationClose: 'top',
                                buttons: {
                                    cancel: {
                                        text: "OK",
                                        btnClass: 'btn-success',
                                        keys: ['enter'],
                                        action: function () {
                                            window.location.href = "/fitness/Index/" + data.PlaceId;
                                        }
                                    }
                                },
                                onContentReady: function () {
                                }
                            });

                        }


                    }
                    else {
                        var audio = new Audio('../../audio/rave_digger.mp3');
                        audio.play();
                        $.alert("User credentials invalid");

                    }
                }
            }).done(function () {

            });

            self.scans.unshift({ date: +(Date.now()), content: content });
        });
        Instascan.Camera.getCameras().then(function (cameras) {
            self.cameras = cameras;
            if (cameras.length > 1)
            {
                self.activeCameraId = cameras[0].id;
                self.scanner.start(cameras[0]);
            }
            else if (cameras.length === 1) {
                self.activeCameraId = cameras[0].id;
                self.scanner.start(cameras[0]);
            }
            else {
                console.error('No cameras found.');
            }
        }).catch(function (e) {
            console.error(e);
        });
    },
    methods: {
        formatName: function (name) {
            return name || '(unknown)';
        },
        selectCamera: function (camera) {
            this.activeCameraId = camera.id;
            this.scanner.start(camera);
        }
    }
});
