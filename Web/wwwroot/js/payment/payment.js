var { createApp } = Vue;

const app2 = createApp({
    data: function () {
        return {
            paymethod: "LINE Pay",
            pay_status: 1,
            step: [
                {
                    step: 1,
                    here: true,
                    "bg-primary": true
                },
                {
                    step: 2,
                    here: false,
                    "bg-primary": false
                },
                {
                    step: 3,
                    here: false,
                    "bg-primary": false
                }

            ],

            second_step: {
                here: false,
                "bg-primary": false
            },
            third_step: {
                here: false,
                "bg-primary": false
            },
        };
    },
    methods: {
        change_status: function () {
            if (this.pay_status < 3) {
                this.pay_status++;
            } else {
                this.pay_status = 1;
            }
        }, handleSubmit: function () {
          
            console.log("handleSubmit triggered");
            if (this.paymethod === "LINE Pay") {
                console.log('in')
                // 將導向到 LinePay/Payment      
                window.location.href = '/LinePay/Payment';
            } else {
              
                console.log(`Selected payment method: ${this.paymethod}`);
                
            }
        }
    },
    computed: {},
    watch: {
        pay_status: {
            handler(newValue, oldValue) {
                this.step.forEach(element => {
                    if (element.step == newValue) {
                        element.here = true;
                        element["bg-primary"] = true;
                    } else {
                        element.here = false;
                        element["bg-primary"] = false;
                    }
                });



            },
            deep: true,
            immediate: true,
        },
    },
});
app2.mount("#app_payment");
