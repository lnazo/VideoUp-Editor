var mainTable, t;

function prepareTable() {
	mainTable = document.getElementById('di-mainTable');
	mainTable.className = "di-sortable";

	for(i=0; i < mainTable.getElementsByTagName("TH").length; i++) {
		var headValue = mainTable.getElementsByTagName("TH")[i].innerHTML;
		mainTable.getElementsByTagName("TH")[i].innerHTML = "<div>" + headValue + "</div>";
	}

	t = new SortableTable(mainTable);
}

function SortableTable(tableEl) {
	this.tbody = tableEl.getElementsByTagName('tbody');
	this.thead = tableEl.getElementsByTagName('thead');
	this.tfoot = tableEl.getElementsByTagName('tfoot');

	this.getInnerText = function(el) {
		if (typeof(el.textContent) != 'undefined') return el.textContent;
		if (typeof(el.innerText) != 'undefined') return el.innerText;
		if (typeof(el.innerHTML) == 'string') return el.innerHTML.replace(/<[^<>]+>/g,'');
	}

	this.getParent = function(el, pTagName) {
		if (el == null) return null;
		else if (el.nodeType == 1 && el.tagName.toLowerCase() == pTagName.toLowerCase())
			return el;
		else
			return this.getParent(el.parentNode, pTagName);
	}

	this.sort = function(cell) {
	    var column = cell.cellIndex;
	    var itm = this.getInnerText(this.tbody[0].rows[1].cells[column]);
		var sortfn = this.sortCaseInsensitive;

		if (itm.match(/^(\d\d?)[\/\.-](\d\d?)[\/\.-]((\d\d)?\d\d)(\s\d\d[\.:]\d\d)?$/)) sortfn = this.sortDate;
		if (itm.match(/^([\d\.,]+)\s*(b|kb|mb|gb|tb)/i)) sortfn = this.sortSize;
		if (itm.replace(/^\s+|\s+$/g,"").match(/^[\d\.]+$/)) sortfn = this.sortNumeric;

		this.sortColumnIndex = column;
		cells = document.getElementsByTagName('TD');
		for (i=0;i<cells.length;i++) {
			removeClass(cells[i], "di-activeColumn");
		}
		
		var zebra = "di-odd";
		
		for (i=0;i<this.tbody[0].rows.length;i++) {
			addClass(this.tbody[0].rows[i].cells[column], "di-activeColumn");
		}
		
	    var newRows = new Array();
	    for (i=0;i<this.tbody[0].rows.length;i++) {
			newRows[i] = this.tbody[0].rows[i];
		}
		newRows.sort(sortfn);

		if (document.getElementsByClassName == undefined) {
			document.getElementsByClassName = function(className) {
				var hasClassName = new RegExp("(?:^|\\s)"+className+"(?:$|\\s)");
				var allElements = document.getElementsByTagName("*");
				var results = [];
				var element;
				for (i=0;(element=allElements[i])!=null;i++) {
					var elementClass = element.className;
					if (elementClass && elementClass.indexOf(className) != -1 && hasClassName.test(elementClass)) results.push(element);
				}
				return results;
			}
		}

		var arrows = document.getElementsByClassName('di-tableSortArrow');
		for (i=0;i<arrows.length;i++) {
			var arrowParent = arrows[i].parentNode;
			arrowParent.removeChild(arrows[i]);
		}
		var spanEl = document.createElement('span');
		spanEl.className = 'di-tableSortArrow';
		if (cell.getAttribute('sortdir') == 'down') {
			newRows.reverse();
			spanEl.appendChild(document.createTextNode(' \u25bc'));
			cell.setAttribute('sortdir','up');
		} else {
			cell.setAttribute('sortdir','down');
			spanEl.appendChild(document.createTextNode(' \u25b2'));
		}

		cell.getElementsByTagName('DIV')[0].appendChild(spanEl);
		for (i=0;i<newRows.length;i++) {
			this.tbody[0].appendChild(newRows[i]);
		}
	}

	this.sortCaseInsensitive = function(a,b) {
		aa = thisObject.getInnerText(a.cells[thisObject.sortColumnIndex]).toLowerCase();
		bb = thisObject.getInnerText(b.cells[thisObject.sortColumnIndex]).toLowerCase();
		if (aa==bb) return 0;
		if (aa<bb) return -1;
		return 1;
	}

	this.sortDate = function(a,b) {
		aa = thisObject.getInnerText(a.cells[thisObject.sortColumnIndex]);
		bb = thisObject.getInnerText(b.cells[thisObject.sortColumnIndex]);
		date1 = aa.substr(6,4)+aa.substr(3,2)+aa.substr(0,2)+aa.substr(11,2)+aa.substr(14,2);
		date2 = bb.substr(6,4)+bb.substr(3,2)+bb.substr(0,2)+bb.substr(11,2)+bb.substr(14,2);
		if (date1==date2) return 0;
		if (date1<date2) return -1;
		return 1;
	}

	this.sortSize = function(a,b) {
		aa = thisObject.getInnerText(a.cells[thisObject.sortColumnIndex]);
		bb = thisObject.getInnerText(b.cells[thisObject.sortColumnIndex]);
		regex = /^([\d\.,]+)\s*(b|kb|mb|gb|tb)/i;
		mcA = aa.match(regex);
		mcB = bb.match(regex);
		ssA = mcA[2].toLowerCase();
		ssB = mcB[2].toLowerCase();

		if (ssA=='b') valA = 1;
		else if (ssA=='kb') valA = 2;
		else if (ssA=='mb') valA = 3;
		else if (ssA=='gb') valA = 4;
		else if (ssA=='tb') valA = 5;
		if (ssB=='b') valB = 1;
		else if (ssB=='kb') valB = 2;
		else if (ssB=='mb') valB = 3;
		else if (ssB=='gb') valB = 4;
		else if (ssB=='tb') valB = 5;

		if (valA==valB) {
			return mcA[1] - mcB[1];
		} else if (valA<valB) {
			return -1;
		} else if (valA>valB) {
			return 1;
		}
	}

	this.sortNumeric = function(a,b) {
		aa = parseFloat(thisObject.getInnerText(a.cells[thisObject.sortColumnIndex]));
		bb = parseFloat(thisObject.getInnerText(b.cells[thisObject.sortColumnIndex]));
		if (isNaN(aa)) aa = 0;
		if (isNaN(bb)) bb = 0;
		return aa-bb;
	}

	var thisObject = this;
	var sortSection = this.thead;
	if (!(this.tbody && this.tbody[0].rows && this.tbody[0].rows.length > 0)) return;
	if (sortSection && sortSection[0].rows && sortSection[0].rows.length > 0) {
		var sortRow = sortSection[0].rows[0];
	} else {
		return;
	}
	for (i=0;i<sortRow.cells.length;i++) {
		sortRow.cells[i].sTable = this;
		sortRow.cells[i].onclick = function () {
			this.sTable.sort(this);
			theads = this.parentNode.getElementsByTagName('TH');
			for (j=0; j < theads.length; j++) {
				removeClass(theads[j], "di-active");
			}
			addClass(this, "di-active");
			zebraStripes();
			return false;
		}
		
	}
}

var zebraStripes = function () {
	var zebra = "di-odd";
	var counter = 0;
	for (var r = 1; r < mainTable.rows.length; r++) {
		if(mainTable.rows[r].style.display == '') {
			if(counter == 0) zebra = "di-odd";
			mainTable.rows[r].className = zebra;
			(zebra == "di-odd") ? zebra = "di-even" : zebra = "di-odd";
			counter++;
		}
	}
}

function hasClass(ele,cls) {
	return ele.className.match(new RegExp('(\\s|^)'+cls+'(\\s|$)'));
}

function addClass(ele,cls) {
	if (!this.hasClass(ele,cls)) ele.className += " "+cls;
}

function removeClass(ele,cls) {
	if (hasClass(ele,cls)) {
    	var reg = new RegExp('(\\s|^)'+cls+'(\\s|$)');
		ele.className=ele.className.replace(reg,' ');
	}
}

addLoadEvent(function() {
	prepareTable();
	zebraStripes();
	}
);