<template>
  <b-container>
    <h1 v-t="'realms.title'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" href="/create-realm" icon="plus" text="actions.create" variant="success" />
    </div>
    <b-row>
      <search-field class="col" v-model="search" />
      <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
      <count-select class="col" v-model="count" />
    </b-row>
    <template v-if="realms.length">
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'realms.alias.label'" />
            <th scope="col" v-t="'realms.displayName.label'" />
            <th scope="col" v-t="'updated'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="realm in realms" :key="realm.id">
            <td>
              <b-link :href="`/realms/${realm.id}`">{{ realm.alias }}</b-link>
            </td>
            <td v-text="realm.displayName || realm.alias" />
            <td><status-cell :actor="realm.updatedBy" :date="realm.updatedOn || realm.createdOn" /></td>
            <td>
              <icon-button icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`delete_${realm.id}`" />
              <delete-modal
                confirm="realms.delete.confirm"
                :displayName="realm.displayName || realm.alias"
                :id="`delete_${realm.id}`"
                :loading="loading"
                title="realms.delete.title"
                @ok="onDelete(realm, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </template>
    <p v-else v-t="'realms.empty'" />
  </b-container>
</template>

<script>
import { deleteRealm, getRealms } from '@/api/realms'

export default {
  name: 'RealmList',
  data() {
    return {
      count: 10,
      desc: false,
      loading: false,
      page: 1,
      realms: [],
      search: null,
      sort: 'DisplayName',
      total: 0
    }
  },
  computed: {
    params() {
      return {
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('realms.sort.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  },
  methods: {
    async onDelete({ id }, callback = null) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await deleteRealm(id)
          refresh = true
          this.toast('success', 'realms.delete.success')
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
      if (callback) {
        callback()
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getRealms(params ?? this.params)
          this.realms = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(newValue, oldValue) {
        if (newValue?.index && oldValue && (newValue.search !== oldValue.search || newValue.count !== oldValue.count)) {
          this.page = 1
          await this.refresh()
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
