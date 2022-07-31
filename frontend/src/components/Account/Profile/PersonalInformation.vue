<template>
  <div>
    <h3 v-t="'user.information.personal'" />
    <table class="table table-striped">
      <tbody>
        <tr v-if="profile.fullName">
          <th scope="row" v-t="'user.fullName'" />
          <td v-text="profile.fullName" />
        </tr>
        <tr v-if="profile.email">
          <th scope="row" v-t="'user.email.label'" />
          <td>
            {{ profile.email }}
            <b-badge v-if="profile.isEmailConfirmed" variant="info">{{ $t('user.email.confirmed') }}</b-badge>
          </td>
        </tr>
        <tr v-if="profile.phoneNumber">
          <th scope="row" v-t="'user.phone.label'" />
          <td>
            {{ profile.phoneNumber }}
            <b-badge v-if="profile.isPhoneNumberConfirmed" variant="info">{{ $t('user.phone.confirmed') }}</b-badge>
          </td>
        </tr>
        <tr>
          <th scope="row" v-t="'user.createdAt'" />
          <td>{{ $d(new Date(profile.createdAt), 'medium') }}</td>
        </tr>
        <tr v-if="profile.updatedAt">
          <th scope="row" v-t="'user.updatedAt'" />
          <td>{{ $d(new Date(profile.updatedAt), 'medium') }}</td>
        </tr>
        <tr v-if="profile.signedInAt">
          <th scope="row" v-t="'user.signedInAt'" />
          <td>{{ $d(new Date(profile.signedInAt), 'medium') }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  name: 'PersonalInformation',
  props: {
    profile: {
      type: Object,
      required: true
    }
  }
}
</script>
