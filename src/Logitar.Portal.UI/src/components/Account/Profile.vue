<template>
  <b-container>
    <h1 v-t="'user.profile.title'" />
    <personal-information :user="user" />
    <authentication-information :user="user" @updated="setModel" />
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
    json: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      user: null
    }
  },
  methods: {
    setModel(user) {
      this.user = user
    }
  },
  created() {
    this.setModel(JSON.parse(this.json))
  }
}
</script>
