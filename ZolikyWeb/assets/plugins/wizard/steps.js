$(function() {
	$(".tab-wizard").steps({
		headerTag: "h6",
		bodyTag: "section",
		transitionEffect: "fade",
		enableAllSteps: true,
		enableCancelButton: false,
		enableFinishButton: true,
		showFinishButtonAlways: false,
		titleTemplate: '<span class="step">#index#</span> #title#',
		labels: {
			next: "Další",
			previous: "Zpět",
			finish: "Hotovo",
			loading: "Načítání"
		},
		onFinished: function(event, currentIndex) {
			const url = window.location.href;
			const last = url.lastIndexOf("Mobile");
			window.location.href = url.substring(0, last);
		}
	});
});