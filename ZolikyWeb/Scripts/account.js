$(() => {
	/* Background animations */
	var animated = Cookies.get("animatedBackground");
	let $bgSwitch = $("#animatedBgSwitch");
	let $wrapper = $("#accountWrapper");

	if (animated === undefined) {
		animated = $wrapper.hasClass("custom-gradient");
	}
	$bgSwitch.prop("checked", animated === "true");
	checkBg($bgSwitch);

	$bgSwitch.change(function() {
		let $t = $(this);
		checkBg($t);
	});

	function checkBg($t) {
		if ($t.is(":checked")) {
			$wrapper.addClass("custom-gradient");
		} else {
			$wrapper.removeClass("custom-gradient");
		}
	}

	$("#settingModelSave").click(function() {
		Cookies.set("animatedBackground", $bgSwitch.is(":checked"));
	});
	/* /Background animations */

	/* Pride Logo */
	var pride = Cookies.get("prideLogo");
	let $logoSwitch = $("#prideLogoSwitch");
	let $logo = $(".zoliky-logo:not('.pride')");
	let $logoPride = $(".zoliky-logo.pride");
	if (pride === undefined) {
		pride = $logoPride.is(":visible").toString();
	}
	$logoSwitch.prop("checked", pride === "true");
	checkLogo($logoSwitch);

	$logoSwitch.change(function() {
		let $t = $(this);
		checkLogo($t);
	});

	function checkLogo($t) {
		if ($t.is(":checked")) {
			$logoPride.show();
			$logo.hide();
		} else {
			$logoPride.hide();
			$logo.show();
		}
	}

	$("#settingModelSave").click(function() {
		Cookies.set("prideLogo", $logoSwitch.is(":checked"));
	});
	/* /Pride logo */

	/* Login (Social) */
	$("form.soc-buttons").submit(() => {
		loading();
	});
	/* /Login (Social) */

	/* Snowing */
	$.snowfall.start({
		content: '<i class="fas fa-snowflake"></i>',
	});
	/* /Snowing */

});
let currentLogin = false;

function login() {

	const $btnLogin = $("button.g-recaptcha");
	const $socButtons = $(".btn-social");
	changeBtn($btnLogin, true);
	changeBtn($socButtons, true);
	$socButtons.addClass("disabled");
	if (currentLogin) {
		return;
	}
	currentLogin = true;

	const $lBar = $("#loadingBar");

	$lBar.show();
	if ($("#loginForm").valid()) {
		$("#loginForm").submit();
		return;
	}
	grecaptcha.reset();
	changeBtn($btnLogin, false);
	changeBtn($socButtons, false);
	$socButtons.removeClass("disabled");
	$lBar.hide();
	grecaptcha.reset();
	currentLogin = false;

}

function gExpire() {
	if (!currentLogin) {
		return;
	}
	const $btnLogin = $("button.g-recaptcha");
	changeBtn($btnLogin, true);
}