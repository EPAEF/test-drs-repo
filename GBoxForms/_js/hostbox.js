// JavaScript document  
// check location
//
// document.write ("hash: " + location.hash);
// document.write (" host: " + location.host);
// document.write (" hostname: " + location.hostname);
// document.write (" href: " + location.href);
// document.write (" pathname: " + location.pathname);
// document.write (" port: " + location.port);
// document.write ("hostname: ", loc);
//
var locD = "D"
var locQ = "Q"
var locP = "G"
var locIS = ""

function abc(){
	var locH = location.host;
	var locHName = location.hostname;
	
	if (locH == "by-bdc.bayer-ag.com"){
		message="Du bist entweder auf D oder Q. Bitte oben mal nachsehen.";
		DQ();
	}
    else if (locH == "by-dbox.bayer-ag.com") {
        message = "We are on D server";
        locIS = locD;
        document.write(" | " + locIS + "_BOX");
        document.title = ("Welcome on " + locIS + "_BOX ");
    }
    else if (locH == "by-qbox.bayer-ag.com") {
        message = "We are on Q server";
        locIS = locQ;
        document.write(" | " + locIS + "_BOX");
        document.title = ("Welcome on " + locIS + "_BOX ");
    }
    else if (locH == "by-gbox.bayer-ag.com") {
        message = "We are on G server";
        locIS = locP;
        document.write(" | " + locIS + "_BOX");
        document.title = ("Welcome on " + locIS + "_BOX ");
    }
    else if ((locH == "sp-appl-bbs.bayer-ag.com") || (locH == "by-gbox.bayer-ag.com")) {
		message="We are on P server";
		locIS = locP;
        document.write(" | " + locIS + "_BOX"); 
        document.title = ("Welcome on " + locIS + "_BOX ");
	}
	else if (locH == "localhost"){
		message="Localhost";
		locIS = (locD + "/" + locH);
        document.write(" | " + locIS); 
        document.title = ("Welcome on " + locIS);
	}
	else{
	    if (locHName == "localhost"){
		    message="Local part.";
		    locIS = "Localhost";
		    document.write(" | " + locIS); 
		    document.title = ("Welcome on " + locIS);	    
		}
	    else{
		    message="No idea where we are.";
		    locIS = "?";
        document.write(" | " + locIS + "_BOX"); 
        document.title = ("Welcome on G|BOX ");
		}
	}
//alert(message);
}


function DQ(){
	var pathname = location.pathname;
	var filename = pathname;
	for(i=0; i<2; i++){
	filename = (filename.substr(filename.indexOf('/')+1,filename.length));
	}
	
	var myString = filename ;
	var mySplitResult = myString.split("/");
	for(i = 0; i < 1; i++){
	mySRI = mySplitResult[i];
	}
	//alert(mySRI);
	if(mySRI == "MDRSDev"){
		locIS = locD;
		document.write(" | " + locIS + "_BOX."); 
		document.title = ("Welcome on " + mySRI + "_BOX ");
	}else if(mySRI == "MDRS"){
		locIS = locQ;
		document.write("| " + locIS + "_BOX"); 
		document.title = ("Welcome on " + mySRI + "Q_BOX ");
	}else {
		locIS = "?";
		document.write(" | " + locIS + "_BOX"); 
		document.title = ("Welcome on " + mySRI + "_BOX ");
	}
}

function LocationCheckWriter(){
	var locH = location.host
	var locHREF = location.href
	var locPN = location.pathname

	document.write ("<br />host: " + locH);
	document.write ("<br />href: " + locHREF);
	document.write ("<br />pathname: " + locPN);

	var pathname = location.pathname;
	var filename = pathname;
	for(i=0; i<2; i++){
	filename = (filename.substr(filename.indexOf('/')+1,filename.length));
	}
	
	var myString = filename ;
	var mySplitResult = myString.split("/");
	for(i = 0; i < 1; i++){
	mySRI = mySplitResult[i];
	document.write ("<br />" + mySRI); 
	}
	
	if(mySRI == "MDRSDev"){
		locIS = locD;
		document.write("<h1>" + locIS + "_BOX" + "</h1>"); 
		document.title = ("Welcome on " + mySRI + "_BOX");
	}else if(mySRI = "MDRS"){
		locIS == locQ;
		document.write("<h1>" + locIS + "_BOX" + "</h1>"); 
		document.title = ("Welcome on " + mySRI + "Q_BOX. ");
	}else {
		locIS = "?";
		document.write("<h1>" + locIS + "_BOX" + "</h1>"); 
		document.title = ("Welcome on " + mySRI + "_BOX. ");
	}

}
