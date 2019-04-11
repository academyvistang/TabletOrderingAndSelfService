// Initialize your app
var myApp = new Framework7({
    animateNavBackIcon: true,
    // Enable templates auto precompilation
    precompileTemplates: true,
    // Enabled pages rendering using Template7
	swipeBackPage: false,
	swipePanelOnlyClose: true,
	pushState: true,
    template7Pages: true
});

// Export selectors engine
var $$ = Dom7;

// Add main View
var mainView = myApp.addView('.view-main', {
    // Enable dynamic Navbar
    dynamicNavbar: false,
});
var subnaview = myApp.addView('.view-subnav');


$(document).ready(function() {
		$("#RegisterForm").validate();
		$("#LoginForm").validate();
		$("#ForgotForm").validate();
		$(".close-popup").click(function() {					  
			$("label.error").hide();
		});
		$('.close_info_popup').click(function(e){
			$('.info_popup').fadeOut(500);						  
		});

		$('.noajax').unbind('click');

		$('.noajax').click(function (e) {
		    var href = $(this).attr("href");
		    e.preventDefault();
		    e.stopPropagation();
		    window.location.href = href;
		});
});


$$(document).on('pageInit', function (e) {
		$("#RegisterForm").validate();
		$("#LoginForm").validate();
		$("#ForgotForm").validate();
		$(".close-popup").click(function() {					  
			$("label.error").hide();
		});

	
})
myApp.onPageInit('music', function (page) {
		  audiojs.events.ready(function() {
			var as = audiojs.createAll();
		  });
})

myApp.onPageInit('InstaScan', function (page)
{
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
            self.scanner.addListener('scan', function (content, image)
            {
                $("a.resultLink").prop("href", content);

                $("#QRRESULT").toggle();



                $.confirm({
                    title: 'SIGN IN SUCCESSFUL',
                    content: '<br><centre><img style="padding-left:0%;" src="../../images/tick-308750_960_720.png"></centre>',
                    type: 'green',
                    animation: 'zoom',
                    animationClose: 'top',
                    buttons: {
                        cancel: {
                            text: "Close",
                            btnClass: 'btn-danger',
                            keys: ['enter'],
                            action: function () {

                            }
                        }
                    },
                    onContentReady: function () {
                    }
                });

                //window.location.href = content;

                window.location.href = "/fitness/index/1";

                 //alert(content);
                self.scans.unshift({ date: +(Date.now()), content: content });
            });
            Instascan.Camera.getCameras().then(function (cameras) {
                self.cameras = cameras;
                if (cameras.length > 0) {
                    self.activeCameraId = cameras[0].id;
                    self.scanner.start(cameras[0]);
                } else {
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
            displayValue: function (content)
            {
                window.location.href = content;
            },
            selectCamera: function (camera) {
                this.activeCameraId = camera.id;
                this.scanner.start(camera);
            }

        }
    });
})



myApp.onPageInit('videos', function (page) {
		  $(".videocontainer").fitVids();
})
myApp.onPageInit('contact', function (page) {
		$("#ContactForm").validate({
		submitHandler: function(form) {
		ajaxContact(form);
		return false;
		}
		});	
})

//SelectMember
myApp.onPageInit('formMember', function (page)
{
   
   

    $('#FeedBackSubmit').click(function (e)
    {
        $('#myDivToBeHidden').show();
        e.preventDefault();
        e.stopPropagation();

        var id = $('#Id').val();
        var placeId = $('#PlaceId').val();

        location.href = "/Fitness/ExistingMembership/" + id + "?placeId=" + placeId;

       
    });


})
myApp.onPageInit('form', function (page)
{
   

    $('#FeedBackSubmit').click(function (e)
    {
       
        $('#myDivToBeHidden').show();

    });

    
    //$("#CustomForm").validate({
    //    rules: {         
    //        selectoptions: {
    //            required: true
    //        }
    //    },
    //    messages: {
    //        selectoptions: "Please select one option"
    //    }
    //});
	//var calendarDefault = myApp.calendar({
	//	input: '#calendar-input',
	//});

	//$('.datetimepicker1').datetimepicker();

		
})


myApp.onPageInit('blog', function (page) {
 
		$(".posts li").hide();	
		size_li = $(".posts li").size();
		x=4;
		$('.posts li:lt('+x+')').show();
		$('#loadMore').click(function () {
			x= (x+1 <= size_li) ? x+1 : size_li;
			$('.posts li:lt('+x+')').show();
			if(x == size_li){
				$('#loadMore').hide();
				$('#showLess').show();
			}
		});

})

myApp.onPageInit('formTableSms', function (page) {





    $("#CustomForm").validate({
        rules: {
            selectoptions: {
                required: true
            }
        },
        messages: {
            selectoptions: "Please select one option"
        }
    });
    var calendarDefault = myApp.calendar({
        input: '#calendar-input',
    });




    $("#SnapSMS").click(function (e) {


        var guestName = $("#GuestName").val();
        var telephone = $("#Telephone").val();
        var guestTelephone = $("#GuestTelephone").val();



        if (guestName.length == 0) {
            $.alert("Please enter your PIN CODE");
            return false;
        }

        if (telephone.length == 0) {
            $.alert("Please enter at least one number for now, numbers to be mined from database");
            return false;
        }

        if (guestTelephone.length == 0) {
            $.alert("Please enter your message");
            return false;
        }



        $("#myDivToBeHidden").show();
    });


})

myApp.onPageInit('formTableSnap', function (page) {





    $("#CustomForm").validate({
        rules: {
            selectoptions: {
                required: true
            }
        },
        messages: {
            selectoptions: "Please select one option"
        }
    });
    var calendarDefault = myApp.calendar({
        input: '#calendar-input',
    });


    $("#SnapDelete").click(function (e) {


        var tableId = $("#TableId").val();

        if (tableId.length == 0) {
            $.alert("Please enter a table Number");
            return false;
        }

        $("#myDivToBeHidden").show();
    });

    $("#SnapClick").click(function (e) {


        var tableId = $("#TableId").val();
        var myFiles = $('#cameraInput').prop('files');


        if (tableId.length == 0) {
            $.alert("Please enter a table Number");
            return false;
        }

        if (myFiles.length == 0) {
            $.alert("Please you must include a receipt image!");
            return false;
        }

        $("#myDivToBeHidden").show();
    });


})

myApp.onPageInit('shop', function (page) {
			
		$('.qntyplusshop').click(function(e){
									  
			e.preventDefault();
			var fieldName = $(this).attr('field');
			var currentVal = parseInt($('input[name='+fieldName+']').val());
			if (!isNaN(currentVal)) {
				$('input[name='+fieldName+']').val(currentVal + 1);
			} else {
				$('input[name='+fieldName+']').val(0);
			}
			
		});
		$(".qntyminusshop").click(function(e) {
			e.preventDefault();
			var fieldName = $(this).attr('field');
			var currentVal = parseInt($('input[name='+fieldName+']').val());
			if (!isNaN(currentVal) && currentVal > 0) {
				$('input[name='+fieldName+']').val(currentVal - 1);
			} else {
				$('input[name='+fieldName+']').val(0);
			}
		});	
  
})
myApp.onPageInit('shopitem', function (page) {
		$(".swipebox").swipebox();	
		$('.qntyplusshop').click(function(e){
									  
			e.preventDefault();
			var fieldName = $(this).attr('field');
			var currentVal = parseInt($('input[name='+fieldName+']').val());
			if (!isNaN(currentVal)) {
				$('input[name='+fieldName+']').val(currentVal + 1);
			} else {
				$('input[name='+fieldName+']').val(0);
			}
			
		});
		$(".qntyminusshop").click(function(e) {
			e.preventDefault();
			var fieldName = $(this).attr('field');
			var currentVal = parseInt($('input[name='+fieldName+']').val());
			if (!isNaN(currentVal) && currentVal > 0) {
				$('input[name='+fieldName+']').val(currentVal - 1);
			} else {
				$('input[name='+fieldName+']').val(0);
			}
		});	
  
})
myApp.onPageInit('cart', function (page) {
			
    $('.item_delete').click(function(e){
        e.preventDefault();
        var currentVal = $(this).attr('id');
        $('div#'+currentVal).fadeOut('slow');
    });
  
})
myApp.onPageInit('photos', function (page) {
	$(".swipebox").swipebox();
	$("a.switcher").bind("click", function(e){
		e.preventDefault();
		
		var theid = $(this).attr("id");
		var theproducts = $("ul#photoslist");
		var classNames = $(this).attr('class').split(' ');
		
		
		if($(this).hasClass("active")) {
			// if currently clicked button has the active class
			// then we do nothing!
			return false;
		} else {
			// otherwise we are clicking on the inactive button
			// and in the process of switching views!

  			if(theid == "view13") {
				$(this).addClass("active");
				$("#view11").removeClass("active");
				$("#view11").children("img").attr("src","images/switch_11.png");
				
				$("#view12").removeClass("active");
				$("#view12").children("img").attr("src","images/switch_12.png");
			
				var theimg = $(this).children("img");
				theimg.attr("src","images/switch_13_active.png");
			
				// remove the list class and change to grid
				theproducts.removeClass("photo_gallery_11");
				theproducts.removeClass("photo_gallery_12");
				theproducts.addClass("photo_gallery_13");

			}
			
			else if(theid == "view12") {
				$(this).addClass("active");
				$("#view11").removeClass("active");
				$("#view11").children("img").attr("src","images/switch_11.png");
				
				$("#view13").removeClass("active");
				$("#view13").children("img").attr("src","images/switch_13.png");
			
				var theimg = $(this).children("img");
				theimg.attr("src","images/switch_12_active.png");
			
				// remove the list class and change to grid
				theproducts.removeClass("photo_gallery_11");
				theproducts.removeClass("photo_gallery_13");
				theproducts.addClass("photo_gallery_12");

			} 
			else if(theid == "view11") {
				$("#view12").removeClass("active");
				$("#view12").children("img").attr("src","images/switch_12.png");
				
				$("#view13").removeClass("active");
				$("#view13").children("img").attr("src","images/switch_13.png");
			
				var theimg = $(this).children("img");
				theimg.attr("src","images/switch_11_active.png");
			
				// remove the list class and change to grid
				theproducts.removeClass("photo_gallery_12");
				theproducts.removeClass("photo_gallery_13");
				theproducts.addClass("photo_gallery_11");

			} 
			
		}

	});	
})

myApp.onPageInit('chat', function (page) {
// Conversation flag
var conversationStarted = false;
 
// Init Messages
var myMessages = myApp.messages('.messages', {
  autoLayout:true
});
 
// Init Messagebar
var myMessagebar = myApp.messagebar('.messagebar');
 
// Handle message
$$('.messagebar .link').on('click', function () {
  // Message text
  var messageText = myMessagebar.value().trim();
  // Exit if empy message
  if (messageText.length === 0) return;
 
  // Empty messagebar
  myMessagebar.clear()
 
  // Random message type
  var messageType = (['sent', 'received'])[Math.round(Math.random())];
 
  // Avatar and name for received message
  var avatar, name;
  if(messageType === 'received') {
    avatar = 'http://lorempixel.com/output/people-q-c-100-100-9.jpg';
    name = 'Kate';
  }
  // Add message
  myMessages.addMessage({
    // Message text
    text: messageText,
    // Random message type
    type: messageType,
    // Avatar and name:
    avatar: avatar,
    name: name,
    // Day
    day: !conversationStarted ? 'Today' : false,
    time: !conversationStarted ? (new Date()).getHours() + ':' + (new Date()).getMinutes() : false
  })
 
  // Update conversation flag
  conversationStarted = true;
});  
})           
