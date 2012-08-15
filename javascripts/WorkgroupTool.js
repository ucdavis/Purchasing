// initial load calls
$(function(){
	$(".button").button();
	$("#update-chart").click(function(){UpdateOrgChart();});
	
	InitializeEditable();	
	
	InitializeTemplates();
	
	// Initialize Dialogs
	InitializeOrgDialog();
	InitializeWorkgroupDialog();
	InitializePersonDialog();
	
	// approval calculations
	InitializeApprovalCalculations();
	
	InitializeRemove();
	InitializeMove();
});

// *** Template Functions

function InitializeTemplates() {
	$(".template-btn").click(function(){
		
		var id = $(this).data("id");
			
		$("#org .ui-widget-content").html($("#" + id).html());
	});
}

// *** generic functions

// Enables the editing of list items, inline
function InitializeEditable()
{
	// http://www.yelotofu.com/2009/08/jquery-inline-edit-tutorial/
	$("#org li span").inlineEdit({
		save: function(e, data) {
			$(this).data("id", data.value);
		}
	});
}

// Updates the Org chart
function UpdateOrgChart()
{
	$("#chart").empty();
		
	$("#org .ui-widget-content > ul").each(function(index,item){
		$(item).jOrgChart({chartElement: "#chart"});
	});
}

// create the object to add
function CreateObject(orgName, name, className)
{
	// get the org li element
	var $org = $("#org span.org, #org span.workgroup").filter(function(){ return $(this).data('id') == orgName; }).parent();
						
	// delete button
	var $dlt = $("<div>").addClass("ui-icon ui-icon-closethick");
	
	// is there a ul in there?
	var $childUl  = $org.children("ul");
	if ($childUl.length >= 1)
	{
		var $li = $("<li>").addClass(className);
		$li.append($("<span>").addClass(className).data("id", name).html(name));
		$li.append($dlt);
		$childUl.append($li);
	}
	else
	{
		var $ul = $("<ul>");
		
		var $li = $("<li>").addClass(className);
		$li.append($("<span>").addClass(className).data("id", name).html(name));
		$li.append($dlt);
		
		$ul.append($li);
		$org.append($ul);
	}
}

// *** organization functions

function InitializeOrgDialog() {
	$("#add-org-dialog").dialog({
		autoOpen: false,
		modal: true,
		buttons: {
			save: function(){
				
				//get the values
				var name = $("#orgName").val();
				var orgName = $("#orgAttachTo").val();
				
				CreateObject(orgName, name, "org");
			
				// close the dialog
				$(this).dialog("close");
			},
			cancel: function(){$(this).dialog("close");}
		}
	});
	
	$("#add-org").click(function(){	
		$("#orgName").val("");
	
		// populate the drop downs
		$("#orgAttachTo").empty();
		$("#orgAttachTo").append($("<option>").html("--Select Org--"));
		
		$("#org span.org").each(function(index,item){
			$("#orgAttachTo").append($("<option>").val($(item).html()).html($(item).html()));
		});
	
		// open the dialog
		$("#add-org-dialog").dialog("open");}
	);
}

// *** workgroup functions

function InitializeWorkgroupDialog() {
	$("#add-workgroup-dialog").dialog({
		autoOpen: false,
		modal: true,
		buttons: {
			save: function(){
			
				var name = $("#workgroupName").val();
				var orgName = $("#workgroupAttachTo").val();
				var admin = $("#workgroupAdmin")[0].checked;
			
				var className = "workgroup";
				
				if (admin) className = className + " admin";
				
				CreateObject(orgName, name, className);
			
				$(this).dialog("close");
			},
			cancel: function(){$(this).dialog("close");}
		}
	});
	
	$("#add-workgroup").click(function(){
		$("#workgroupName").val("");
		
		$("#workgroupAttachTo").empty();
		$("#workgroupAttachTo").append($("<option>").html("--Select Org--"));
		
		$("#org span.org").each(function(index,item){
			$("#workgroupAttachTo").append($("<option>").val($(item).html()).html($(item).html()));
		});
		
		$("#add-workgroup-dialog").dialog("open");
	});
}

// *** people functions

function InitializePersonDialog() {

	$("#add-person-dialog").dialog({
		autoOpen: false,
		modal: true,
		buttons: {
			save: function(){
				
				var name = $("#personName").val();
				var orgName = $("#personAttachTo").val();
				var role = $("#personRole").val();
				
				CreatePerson(orgName, name, role);
			
				$(this).dialog("close");
			},
			cancel: function(){$(this).dialog("close");}
		}
	});

	$("#add-person").click(function(){
		$("#personName").val("");
		$("#personAttachTo").empty();
		$("#org span.workgroup").each(function(index,item){
			$("#personAttachTo").append($("<option>").val($(item).html()).html($(item).html()));
		});
		
		$("#personRole").val("");
		
		$("#add-person-dialog").dialog("open");
	});
}

// create the object to add
function CreatePerson(workgroupName, name, className)
{
	// remove any unnecessary spaces
	className = className.replace(/\s/g, '');

	// get the workgroup li element
	var $workgroup = $('#org span.workgroup').filter(function(){ return $(this).data('id') == workgroupName; }).parent(); 
	
	/*
	var $span = $("<span>").addClass(className.toLowerCase()).addClass("person").html(name).data("id", name);
	var $icon = $("<div>").addClass("ui-icon ui-icon-closethick");
	
	$workgroup.append($span);
	$workgroup.append($icon);
	*/
	
	var person = [{role:className.toLowerCase(), name: name}];
	var $span = $('#person-template').tmpl(person).appendTo($workgroup);
}

// *** Approval Calculations
function InitializeApprovalCalculations() {

	$("#approval-dialog").dialog({
		autoOpen: false,
		modal: true,
		buttons: {
			ok: function(){$(this).dialog('close');}
		}
	});

	$('#chart').delegate('.requester', 'click', function () {
	
		var id = $(this).data("id");
	
		// node in org chart
		var $node = $("#org span").filter(function(){ return $(this).data('id') == id;});
		
		debugger;
		
		// roles
		var ap = [];
		var am = [];
		var pr = [];
			
		var test = $node.parents("li.org");
		debugger;
		
		// iterate through all orgs (that this workgroup's org reports to)
		$node.parents("li.org").each(function(index,item){
					
			// look at each org's workgroup
			$(item).children("ul").children("li.workgroup").each(function(index2,item2){
				// workgroup order was placed against
				if ($(item2).data("id") == $node.parent().data("id"))
				{
					parseWorkgroup($(item2), ap, am, pr);
				}
				// admin workgroup that has access
				else if ($(item2).hasClass("admin"))
				{
					parseWorkgroup($(item2), ap, am, pr);
				}
			
			});
		});
		
		addApprovals("#app-approvers", ap);
		addApprovals("#app-accountmanagers", am);
		addApprovals("#app-purchasers", pr);

		$("#approval-dialog").dialog("open");
		
	});

}

function parseWorkgroup ($workgroup, ap, am, pr) {
	$workgroup.children(".approver, .accountmanager, .purchaser").each(function(index,item){
	
		var name = $(item).html() + "(" + $workgroup.children("span.workgroup").data("id") + ")";
	
		if ($(item).hasClass("approver")) {ap.push(name);}
		if ($(item).hasClass("accountmanager")) {am.push(name);}
		if ($(item).hasClass("purchaser")) {pr.push(name);}
	});
}

function addApprovals(listName, approvals) {
	var $list = $(listName);
	$list.empty();
	
	if (approvals.length > 0)
	{
		$.each(approvals, function(index,item){
			$list.append($("<li>").html(item));
		});
	}
	else
	{
		$list.append($("<li>").html("No people found for role."));
	}
}

// *** Remove Functions
function InitializeRemove() {

	$("#org").delegate(".ui-icon-closethick", "click", function(){
		
		var $sib = $(this).prev();
		
		// delete the whole li
		if ($sib.hasClass("workgroup") || $sib.hasClass("org"))
		{
			$(this).parent().remove();
		}
		else if ($sib.hasClass("person"))
		{
			$sib.remove();
			$(this).remove();
		}
				
	});

}

function InitializeMove() {

	$("#move-dialog").dialog({
		autoOpen: false,
		modal: true,
		buttons: {
			save: function(){		
				var $obj = $("#org .selected");
				
				var org = $("#moveAttachTo").val();
				var $orgAttach = $("#org span[data-id='"+org+"']").parent();
				
				var $childUl  = $orgAttach.children("ul");
				if ($childUl.length >= 1)
				{
					$obj.appendTo($childUl);
				}
				else
				{
					var $ul = $("<ul>");
					$orgAttach.append($ul);
					
					$obj.appendTo($ul);
				}
								
				$obj.removeClass("selected");
				$(this).dialog("close");
			},
			cancel: function(){
				var $obj = $("#org .selected");
				$obj.removeClass("selected");
				$(this).dialog("close");
			}
		}
	});

	$("#org").delegate(".ui-icon-transferthick-e-w", "click", function() {
	
		var $sib = $(this).prev().prev();
	
		$("#moveName").html($sib.data("id"));
	
		$("#moveAttachTo").empty();
		$("#moveAttachTo").append($("<option>").html("--Select Org--"));		
		$("#org span.org").each(function(index,item){
			$("#moveAttachTo").append($("<option>").val($(item).html()).html($(item).html()));
		});
	
		$sib.parent().addClass("selected");
	
		$("#move-dialog").dialog("open");
	
	});

}