import Vue from 'vue'
import { ValidationObserver, ValidationProvider, extend, localize } from 'vee-validate'
import { alpha_num, confirmed, email, max, max_value, min, min_value, required } from 'vee-validate/dist/rules'

import './rules/alias'
import './rules/key'
import './rules/requireDigit'
import './rules/requireLowercase'
import './rules/requireNonAlphanumeric'
import './rules/requireUppercase'
import './rules/requiredUniqueChars'
import './rules/url'
import './rules/username'

import en from './en.json'
import fr from './fr.json'

Vue.component('validation-observer', ValidationObserver)
Vue.component('validation-provider', ValidationProvider)

extend('alpha_num', alpha_num)
extend('confirmed', confirmed)
extend('email', email)
extend('max', max)
extend('max_value', max_value)
extend('min', min)
extend('min_value', min_value)
extend('required', required)

localize({ en, fr })
