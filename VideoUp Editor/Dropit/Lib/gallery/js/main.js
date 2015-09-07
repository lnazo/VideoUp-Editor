$(document).ready(function() {

    $('a[data-rel]').each(function() {
        $(this).attr('rel', $(this).data('rel'));
    });

    $('a.title').click(function() {
        $(this).parent().parent().siblings('a.image, a.no-image').trigger('click');
        if($(this).parent().parent().siblings('a.image').length) {
            return false;
        }
    });

    $('a.fancybox').fancybox({
//        prevEffect : 'none',
//        nextEffect : 'none',

        closeBtn    : true,
        arrows      : true,
        nextClick   : true,
        mouseWheel  : true,
        loop        : true,
//        direction   : {
//                        next : 'left',
//                        prev : 'right'
//                    },
//        keys        : {
//                        next : {
//                            13 : 'left', // enter
//                            34 : 'up',   // page down
//                            39 : 'left', // right arrow
//                            40 : 'up'    // down arrow
//                        },
//                        prev : {
//                            8  : 'right',  // backspace
//                            33 : 'down',   // page up
//                            37 : 'right',  // left arrow
//                            38 : 'down'    // up arrow
//                        },
//                        close  : [27] // escape key
//                    },
        helpers     :  {
                        overlay: {
                            locked: false
                        },
                        title	: {
                            type: 'outside'
                        },
                        thumbs      : {
                            width	: 48,
                            height	: 48
                        }
                    }
    });

});