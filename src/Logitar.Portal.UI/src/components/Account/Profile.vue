<template>
  <b-container>
    <h1 v-t="'user.profile.title'" />
    <personal-information :user="user" @updated="setModel" />
    <authentication-information :passwordSettings="passwordSettings" :user="user" @updated="setModel" />
  </b-container>
</template>

<script>
import AuthenticationInformation from './Profile/AuthenticationInformation.vue'
import PersonalInformation from './Profile/PersonalInformation.vue'

export default {
  name: 'Profile',
  components: {
    AuthenticationInformation,
    PersonalInformation
  },
  props: {
    configuration: {
      type: String,
      required: true
    },
    json: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      passwordSettings: null,
      user: null
    }
  },
  methods: {
    setModel(user) {
      this.user = user
    }
  },
  created() {
    const { passwordSettings } = JSON.parse(this.configuration)
    this.passwordSettings = passwordSettings

    this.setModel(JSON.parse(this.json))
  }
}
</script>
