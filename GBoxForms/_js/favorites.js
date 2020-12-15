//------------------------------------------------------
////////////////////////////////////////////////////////
// Code to add website and link to favorite websites 
//------------------------------------------------------
////////////////////////////////////////////////////////
////////////////////////////////////////////////////////
// To check data
// document.write ("hostname " + varHostname);
// document.write ("<br />");
// document.write ("Link: " + varHref);
// document.write ("<br />");
// document.write ("pathname: " + varPath);
// document.write (" href: " + location.href);

var varHostname = location.hostname;
var varHref = location.href;
var varPath = location.pathname;
var fn = location.href;

for(i=0; i<1; i++){
	fn = (fn.substr(fn.indexOf('?')+9,fn.length));
}

var kit = "";
var words = "";

if (fn.indexOf("%20")>0){
    words = fn.split("%20");
    for (j=0;j<words.length;j++){
        kit = kit.concat(words[j] + " ");
    }
}
else if (fn.indexOf("+")>0){  
    words = fn.split("+");
    for (j=0;j<words.length;j++){
        kit = kit.concat(words[j] + " ");
    }
}
else{
    kit = fn;
}

function AddToFavorites() {

    if (varPath == "/index.aspx")
    {
        title = ("GBOX - Bridging your Master Data");
        url = varHref;
    }
    else if (varPath == "/EffortTracking.aspx"){
        title = ("GBOX EffortTracking - " + kit);
        url = varHref;
    }
    else if (varPath == "/Cockpit.aspx"){
        title = ("GBOX COCKPIT");
        url = varHref;
    }
    else if (varPath == "/DynamicForm.aspx"){
        title = ("GBOX DynamicForm");
        url = varHref;
    }
    else if (varPath == "/AutoClass.aspx"){
        title = ("GBOX - Request autoclassification or filter settings");
        url = varHref;
    }
    else{
        title = ("GBOX - Bridging your Master Data " + varPath);
        url = varHref;
    }

    // Check Browser 
    if (window.sidebar) { // Mozilla Firefox Bookmark
        window.sidebar.addPanel(title, url,"");
    }
    else if( window.external ) { // IE Favorite
        window.external.AddFavorite( url, title); 
    }
    else if(window.opera && window.print) { // Opera Hotlist
        return true; 
    }
}
