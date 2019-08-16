(function($) {
	"use strict";


	/*---------------------
	preloader
	--------------------- */

	//$(window).on("load",
	//	() => {
	//		$("#preloader").fadeOut("fast",
	//			() => {
	//				$(this).remove();
	//			});
	//	});

	//=========================
	//  Active current menu while scrolling
	//=========================

	//ACTIVE CURRENT MENU WHILE SCROLLING

	$(window).on("scroll",
		() => {
			activeMenuItem($(".nav-menu"));

		}
	);

	// function for active menuitem
	function activeMenuItem($links) {
		const topA = $(window).scrollTop();
		const footerHeight = $("footer").height();
		const windowHeight = $(window).height();
		const documentHeight = $(document).height() - footerHeight;
		const curPos = topA + 2;
		const sections = $("section");
		const nav = $links;
		const navHeight = nav.outerHeight();
		const home = nav.find(" > ul > li:first");


		sections.each(function() {
			const top = $(this).offset().top - navHeight - 40;
			const bottom = top + $(this).outerHeight();
			if (curPos >= top && curPos <= bottom) {
				nav.find("> ul > li > a").parent().removeClass("active");
				nav.find("a[href='#" + $(this).attr("id") + "']").parent().addClass("active");
				return false;
			} else if (curPos === 2) {
				nav.find("> ul > li > a").parent().removeClass("active");
				home.addClass("active");
			} else if ($(window).scrollTop() + windowHeight > documentHeight - 400) {
				nav.find("> ul > li > a").parent().removeClass("active");
			}
		});
	}

	//=========================
	// Smoth Scroll
	//=========================

	function smoothScrolling($links, $topGap) {
		const links = $links;
		const topGap = $topGap;

		links.on("click",
			function() {
				if (location.pathname.replace(/^\//, "") === this.pathname.replace(/^\//, "") &&
					location.hostname === this.hostname) {
					var target = $(this.hash);
					target = target.length ? target : $("[name=" + this.hash.slice(1) + "]");
					if (target.length) {
						$("html, body").animate({
								scrollTop: target.offset().top - topGap
							},
							1000,
							"easeInOutExpo");
						return false;
					}
				}
				return false;
			});
	}

	$(window).on("load",
		function() {
			smoothScrolling($(".main-menu > nav > ul > li > a[href^='#']"), 70);
			smoothScrolling($(".scroll"), 70);
		});

	//=========================
	// Slick Nav Activation
	//=========================
	$(".nav-menu > ul").slicknav({
		'prependTo': ".mobile_menu"
	});

	/*---------------------
	screen slider
	--------------------- */
	function screenSlider() {
		const owl = $(".screen-slider");
		owl.owlCarousel({
			loop: true,
			margin: 50,
			nav: false,
			items: 3,
			//smartSpeed: 1000,
			//fluidSpeed: 500,
			autoplaySpeed: 1000,
			lazyLoad: true,
			dots: true,
			autoplay: true,
			center: true,
			autoplayTimeout: 6000,
			dotsEach: true,
			responsive: {
				0: {
					items: 1
				},
				480: {
					items: 3
				},
				760: {
					items: 3
				},
				1080: {
					items: 3
				},
				1920: {
					items: 3
				}
			}
		});
	}

	screenSlider();

	/*------------------------------
	     counter
	------------------------------ */

	$(".counter").counterUp({
		delay: 20,
		time: 2000
	});

}(jQuery));