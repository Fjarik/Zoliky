$(() => {
	socSwitcheries.forEach((elm) => {
		$(elm.element).change(function () {
			let $t = $(this);
			if (!$t.is(":checked")) {
				const returnVal = confirm("Opravdu si přejete odebrat toto připojení?");
				if (!returnVal) {
					console.log(false);
					elm.setPosition(true);
					return;
				}
			}
			elm.disable();
			loading();
			$t.closest("form").submit();
		});
	});

	$("form").submit(function() {
		if ($(this).valid()) {
			loading();
		}
	});

	zolikDetails();
	$("#zolikSelect").change(zolikDetails);

});

function zolikDetails() {
	if (!$("#Zoliky") || !$("#Zoliky").val() || !$("#zolikSelect")) {
		return;
	}
	const id = parseInt($("#zolikSelect").children("option:selected").val(), 10);
	const zoliks = JSON.parse($("#Zoliky").val());
	const zolik = zoliks.find(x => x.ID === id);
	if (!zolik) {
		$("#zolikType").text("");
		$("#zolikDate").text("");
		$("#zolikTitle").text("");
		return;
	}
	$("#zolikType").text(zolikTypes[zolik.Type]);
	$("#zolikDate").text(new Date(zolik.OwnerSince).toLocaleDateString());
	$("#zolikTitle").text(zolik.Title);
}