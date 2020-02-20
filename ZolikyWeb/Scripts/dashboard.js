$(() => {
	socSwitcheries.forEach((elm) => {
		$(elm.element).change(function() {
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
	const zolikDetails = () => {
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
		console.log(zolik);
		$("#zolikType").text(zolik.Type);
		$("#zolikDate").text(new Date(zolik.Since).toLocaleDateString());
		$("#zolikTitle").text(zolik.Title);
	};

	zolikDetails();
	$("#zolikSelect").change(zolikDetails);
});