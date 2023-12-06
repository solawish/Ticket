﻿namespace Ticket.Application.Commands.TicketPlus.GenerateRecaptchCommand;

public class GenerateRecaptchDto
{
    //    {
    //    "errCode": "00",
    //    "errMsg": "",
    //    "errDetail": "",
    //    "key": "fetix.1698336036062729",
    //    "data": "<svg xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\" height=\"75\" width=\"213\" viewBox=\"0,0,213,75\"><path d=\"M38.993081 37.490898Q38.724481 37.452527 38.417509 37.452527Q37.074507 37.452527 35.616391 38.296699Q34.273389 39.409472 34.273389 41.328046Q34.273389 44.781479 35.808248 46.124481Q36.767535 47.313997 39.069824 47.313997Q39.338424 47.352368 39.645396 47.352368Q41.372113 47.352368 42.523257 45.702394Q43.559287 44.167535 43.482544 42.364076Q43.559287 41.903618 43.559287 41.481532Q43.559287 39.678072 42.177914 38.507742Q40.796541 37.337412 38.993081 37.490898zM43.520915 47.736083Q42.523257 49.693028 38.762852 49.808143Q35.309419 49.884886 33.697817 48.273283Q32.239701 46.508195 31.4339 41.673389Q31.126928 39.908301 31.126928 38.642042Q31.126928 36.838583 31.855986 35.764182Q33.237359 34.382808 36.230335 34.382808Q42.6 34.382808 44.518574 36.992069Q44.710431 36.186268 45.132518 34.574666Q46.398776 34.267694 48.854551 33.461893Q46.398776 39.140872 46.168547 45.510537Q45.938319 51.726717 48.087121 57.597553Q46.360405 56.983609 44.556945 56.715009Q43.63603 52.724375 43.520915 47.736083zM44.288345 57.021981Q45.017403 57.175466 46.437148 57.482438Q46.667377 58.211496 47.242949 59.631241Q49.852209 60.360299 52.154498 61.664929Q48.087121 55.026664 48.087121 46.431453Q48.087121 40.023416 50.581267 34.190951Q49.890581 34.497923 48.509208 35.150238Q48.816179 34.344437 49.468495 32.771206Q47.933635 33.346778 44.710431 34.229322Q44.595317 34.804895 44.288345 35.99441Q42.254657 34.075837 36.076849 33.922351Q32.853645 33.845608 31.472271 35.342095Q30.704842 36.49324 30.743213 38.411814Q30.858328 41.44316 31.855986 45.241937Q32.508301 47.65934 33.429217 48.618627L33.812931 49.002342Q35.002447 51.803459 40.489569 52.033688Q41.525599 52.07206 43.36743 51.765088Q43.674401 54.719692 44.288345 57.021981zM40.873283 39.793187Q41.986056 39.831558 42.830229 40.13853Q43.022086 40.675731 43.137201 41.289674Q43.213944 41.826875 43.137201 42.440819Q43.137201 44.359393 42.101171 45.664023Q40.950026 47.122139 39.069824 46.930282Q37.765194 46.930282 36.805907 46.508195Q36.460563 45.702394 36.460563 44.474507Q36.422192 44.129164 36.422192 43.822192Q36.422192 42.057104 37.765194 40.886774Q39.108195 39.716444 40.873283 39.793187zM83.108754 49.079085Q80.192522 49.23257 78.926263 48.69537Q76.969318 47.889569 77.046061 45.395423Q77.199547 41.980361 77.199547 42.555933Q77.199547 37.183926 74.321686 32.310749Q76.317003 33.270036 78.31232 33.65375Q80.039036 38.296699 79.923922 42.824534Q79.770436 44.321021 80.614608 45.548909Q81.535524 46.89191 83.070383 46.700053L83.454098 46.584938Q84.797099 46.508195 85.622086 45.740766Q86.447073 44.973336 86.447073 44.014049Q86.447073 43.783821 86.408702 43.591963Q86.331959 38.296699 87.483103 33.922351Q88.519133 33.845608 89.516791 33.615379L91.55048 33.11655Q88.979591 38.181585 89.286562 43.783821Q89.47842 46.89191 87.751703 48.043055Q86.447073 48.925599 83.108754 49.079085zM85.449415 51.496488Q88.941219 51.649974 90.78305 50.882544Q91.703966 50.115114 91.703966 48.69537Q91.703966 47.774454 91.435365 46.201224Q90.974907 43.054762 91.358622 40.023416Q91.742337 36.838583 93.162082 34.075837Q92.509767 34.382808 91.166765 34.804895Q91.396994 34.190951 91.703966 33.615379L92.279538 32.464234Q90.322592 33.11655 87.13776 33.615379Q85.909872 38.450185 86.063358 43.591963Q86.140101 45.702394 83.454098 46.201224L83.108754 46.316338H82.955269L81.995982 46.162852Q81.880867 45.241937 81.919239 44.743107Q82.111096 40.176902 81.420409 35.72581L80.384379 35.610696Q79.847179 35.572324 79.309978 35.45721Q78.926263 34.037465 78.657663 33.270036Q75.856545 32.847949 73.592628 31.543319Q75.741431 35.073495 76.470489 38.488557Q76.969318 40.714102 76.815832 44.244278Q76.854203 44.397764 76.777461 45.510537Q76.623975 47.889569 78.005348 48.848856Q78.389063 51.112773 85.449415 51.496488zM129.10463 38.526928Q128.98952 37.299041 127.03257 36.569983Q126.53374 36.378125 125.99654 36.378125Q124.61517 36.378125 123.54077 36.953697Q122.15939 37.682755 122.27451 38.910643Q122.61985 41.750132 122.46636 44.014049Q122.31288 46.469824 121.58382 49.002342Q119.43502 49.501171 118.2455 49.923257Q119.8571 46.277967 119.8571 42.248961Q119.8571 36.76184 117.24784 32.272377Q118.47573 32.771206 120.89313 33.500264Q121.20011 34.229322 121.58382 35.764182Q122.92682 33.65375 126.84071 33.730493Q128.3372 33.768865 129.143 33.883979Q132.13598 34.267694 132.21272 36.301382Q132.25109 37.222298 132.05923 38.258328Q131.59878 40.790845 131.79063 43.822192Q131.94412 46.700053 132.94178 49.270942Q131.21506 48.848856 129.48835 48.69537Q129.02789 46.316338 128.95114 43.630335Q128.8744 40.867588 129.10463 38.526928zM129.10463 49.040713Q129.91043 49.040713 131.21506 49.270942L131.59878 50.345343Q131.79063 50.844173 132.02086 51.381373Q134.32315 51.803459 137.04753 52.992975Q134.66849 49.846514 133.97781 46.009366Q133.63246 43.860563 133.74758 40.13853Q133.74758 39.831558 133.78595 39.025757Q133.86269 38.411814 133.78595 37.912984Q133.67084 36.224639 132.55806 35.72581Q132.40458 34.881638 131.63715 34.267694Q129.91043 33.231664 126.84071 33.308407Q124.96051 33.346778 124.11634 33.538636Q122.65822 33.883979 121.69893 34.804895Q121.58382 34.267694 121.20011 33.231664Q118.36062 32.540977 116.51879 31.543319Q119.5885 36.454868 119.5885 42.248961Q119.5885 46.661681 117.66993 50.537201Q118.36062 50.230229 119.66525 49.846514Q119.16642 51.112773 118.62922 52.110431Q120.04896 51.649974 123.61751 51.074401Q124.61517 45.894252 124.50005 41.021074Q124.46168 40.176902 125.6512 39.447843Q126.68723 38.795528 127.8 38.680414Q128.26046 38.642042 128.68254 38.8339Q128.52906 41.404789 128.56743 43.975678Q128.6058 46.508195 129.10463 49.040713zM166.60122 36.531611Q164.03033 36.531611 162.9943 37.721127Q161.95827 38.910643 161.95827 41.558275Q161.95827 46.700053 166.60122 46.508195Q168.71165 46.700053 170.07384 45.126822Q171.43603 43.553592 171.2058 41.481532Q171.2058 38.910643 170.32326 37.874613Q168.82677 36.531611 166.60122 36.531611zM171.66626 46.393081Q170.47674 48.887227 165.75705 48.887227H164.29893Q161.9199 48.887227 160.80713 47.736083Q159.7711 46.700053 159.5025 44.28265Q159.34901 42.786162 159.27227 40.982703Q159.2339 39.985044 159.08041 38.028099Q158.9653 33.692122 164.18382 33.692122H165.75705Q168.67328 33.692122 170.78371 34.996752Q171.58952 35.495581 172.08835 36.186268Q172.35695 35.380467 172.89415 33.730493Q174.12203 33.538636 176.46269 32.924692Q173.85343 37.183926 173.85343 42.47919Q173.85343 46.239595 175.23481 49.347685Q174.08366 49.040713 172.16509 48.810484Q171.97323 47.966312 171.66626 46.393081zM175.00458 51.649974L177.23012 52.225546Q178.45801 52.532518 179.45567 52.992975Q175.77201 48.618627 175.77201 42.47919Q175.77201 37.912984 177.92081 34.075837Q177.26849 34.306065 175.92549 34.68978Q176.27084 33.922351 177.15338 32.387492Q176.57781 32.579349 172.5488 33.423521L171.97323 35.265352Q170.16977 33.500264 165.71868 33.270036Q164.60591 33.308407 162.34199 33.461893Q158.61996 33.807236 158.73507 37.874613Q158.85019 41.212931 159.42576 45.702394L159.46413 46.162852Q159.57924 47.007025 160.38504 48.119798Q160.53853 48.273283 160.73039 48.388398Q160.99899 48.887227 161.3827 49.462799Q162.80245 50.997658 166.21751 51.189516Q167.86748 51.30463 168.02097 51.30463H169.5942Q172.8174 51.30463 174.04529 49.7314Q174.39063 50.345343 175.00458 51.649974zM168.40468 38.795528Q169.74768 38.795528 170.59186 39.255986Q170.89883 40.061787 170.89883 41.519903Q171.09069 43.438477 169.80524 44.877408Q168.5198 46.316338 166.60122 46.124481Q165.14311 46.124481 164.10708 45.702394Q163.91522 44.858222 163.91522 43.630335Q164.03033 40.061787 166.67797 39.1025Q167.59888 38.795528 168.40468 38.795528z\" /></svg>"
    //}

    public string ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string ErrDetail { get; set; }

    public string Key { get; set; }

    public string Data { get; set; }
}