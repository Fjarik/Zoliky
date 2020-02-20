window.onload = () => {
	console.log("%cNic sem nevkládejte", "color: red; font-size: x-large;");
};

var socSwitcheries = [];
$(function() {
	$('[data-toggle="tooltip"]').tooltip();

	$("#logOutFormSubmit").click(function() {
		$("#logOutForm").submit();
	});

	$("[data-click='submit']").click(submitClosest);

	$("a[data-lightbox='image']").each(function() {
		$(this).children().attr("src", $(this).attr("href"));
	});

	// Switchery
	const pushSwitchery = (array, $this) => array.push(new Switchery($this[0], $this.data()));
	var switcheries = []; // :-D
	$(".js-switch").each(function () {
		const $this = $(this);
		if ($this.hasClass("soc")) {
			pushSwitchery(socSwitcheries, $this);
		} else {
			pushSwitchery(switcheries, $this);
		}
	});

	$("[data-submitOnChange='True']").change(function() {
		var $elm = $(this);
		let isDisabled = false;
		switcheries.forEach(function(obj) {
			if (obj.element === $elm[0]) {
				obj.disable();
				isDisabled = true;
			}
		});
		if (!isDisabled) {
			$elm.prop("disabled", true);
		}
		loading();
		$elm[0].form.submit();
	});


});

function changeBtn($elm, disabled) {
	$elm.prop("disabled", disabled);
}

function loading() {
	$(".preloader").fadeIn();
}

function submitClosest() {
	$(this).closest('form').submit();
}
